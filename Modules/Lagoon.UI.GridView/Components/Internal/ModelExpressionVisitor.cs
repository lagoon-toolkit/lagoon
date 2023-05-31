using System.Collections;
using System.Runtime.CompilerServices;

namespace Lagoon.UI.Components.Internal;

internal class ModelExpressionVisitor : ExpressionVisitor
{

    #region constants

    public const string PARAMETER_NAME = "d";

    #endregion

    #region private classes

    private sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Default { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    #endregion

    #region fields

    private ParameterExpression _parameter;
    private Dictionary<Type, Dictionary<object, Expression>> _objects = new();
    private Dictionary<Expression, Expression> _quickSearch = new();
    private Type _listType = typeof(IList);
    private Type _dicoType = typeof(IDictionary);
    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="model">Instance of class to visit.</param>
    /// <param name="browseProperties">Indicate if we must register the class instance found in sub properties.</param>
    public ModelExpressionVisitor(object model, bool browseProperties)
    {
        Type type = model.GetType();
        _parameter = Expression.Parameter(type, PARAMETER_NAME);
        AddObjectRecursive(model, type, _parameter, browseProperties);
    }

    #endregion

    #region methods

    /// <summary>
    /// Return an expression to get the bound value with an item parameter.
    /// </summary>
    /// <param name="headerValueExpression">Value expression for the column.</param>
    /// <param name="expectedType">Expected type.</param>
    /// <returns>An expression to get the bound value with an item parameter.</returns>
    public LambdaExpression ToParameterizedLambda(LambdaExpression headerValueExpression, Type expectedType)
    {
        Expression newExpression = Visit(headerValueExpression.Body);
        if(newExpression.Type != expectedType)
        {
            newExpression = Expression.Convert(newExpression, expectedType);
        }
        return Expression.Lambda(newExpression, _parameter);
    }

    public override Expression Visit(Expression node)
    {
        return base.Visit(node);
    }

    /// <summary>
    /// Replace the known member by they known expression.
    /// </summary>
    /// <param name="node">Member expression node.</param>
    /// <returns>The new expression.</returns>
    protected override Expression VisitMember(MemberExpression node)
    {
        if (IsReplaceableExpression(node, out Expression customizedExpression))
        {
            return customizedExpression;
        }
        else if (IsReplaceableExpression(node.Expression, out customizedExpression))
        {
            return node.Update(customizedExpression);
        }
        else
        {
            return base.VisitMember(node);
        }
    }

    /// <summary>
    /// Indicate if the expression target the working model.
    /// </summary>
    /// <param name="originalExpression">The expression.</param>
    /// <param name="customizedExpression">The expression transform for lambda use.</param>
    /// <returns><c>true</c> if if the expression target the working model.</returns>
    private bool IsReplaceableExpression(Expression originalExpression, out Expression customizedExpression)
    {
        if (_quickSearch.TryGetValue(originalExpression, out customizedExpression))
        {
            return true;
        }
        else
        {
            if (_objects.TryGetValue(originalExpression.Type, out Dictionary<object, Expression> dico))
            {
                Expression<Func<object>> currentExpression = Expression.Lambda<Func<object>>(Expression.Convert(originalExpression, typeof(object)));
                object value = currentExpression.Compile()();
                if (dico.TryGetValue(value, out customizedExpression))
                {
                    _quickSearch.Add(originalExpression, customizedExpression);
                    return true;
                }
            }
        }
        customizedExpression = null;
        return false;
    }

    /// <summary>
    /// Keep the expression corresponding to the instance of object.
    /// </summary>
    /// <param name="sourceObject">The instance to keep.</param>
    /// <param name="sourceType">The type of the instance.</param>
    /// <param name="sourceExpression">The expression representing the object.</param>
    /// <param name="browseProperties">Indicate if we must browse the properties of the object.</param>
    private void AddObjectRecursive(object sourceObject, Type sourceType, Expression sourceExpression, bool browseProperties)
    {
        // We ignore primitive types and strings
        if (Type.GetTypeCode(sourceType) != TypeCode.Object)
        {
            return;
        }
        // Keep the expression corresponding to the object reference
        AddObject(sourceObject, sourceType, sourceExpression);
        // Check if we browse the children
        if (!browseProperties)
        {
            return;
        }
        // Check if the property is a dictionary
        if (_dicoType.IsAssignableFrom(sourceType))
        {
            IDictionary dico = (IDictionary)sourceObject;
            PropertyInfo indexer = (from p in sourceType.GetDefaultMembers().OfType<PropertyInfo>()
                                    let q = p.GetIndexParameters()
                                    where q.Length == 1
                                    select p).Single();
            foreach (object key in dico.Keys)
            {
                IndexExpression subItemExpression = Expression.Property(sourceExpression, indexer, Expression.Constant(key));
                object subValue = dico[key];
                if (subValue is not null)
                {
                    AddObjectRecursive(subValue, subValue.GetType(), subItemExpression, browseProperties);
                }
            }
        }
        // Check if the property is a list
        else if (_listType.IsAssignableFrom(sourceType))
        {
            IList list = (IList)sourceObject;
            PropertyInfo indexer = (from p in sourceType.GetDefaultMembers().OfType<PropertyInfo>()
                                    let q = p.GetIndexParameters()
                                    // Here we can search for the exact overload. Length is the number of "parameters" of the indexer, and then we can check for their type.
                                    where q.Length == 1 && q[0].ParameterType == typeof(int)
                                    select p).Single();
            for (int i = 0; i < list.Count; i++)
            {
                IndexExpression subItemExpression = Expression.Property(sourceExpression, indexer, Expression.Constant(i));
                object subValue = list[i];
                if (subValue is not null)
                {
                    AddObjectRecursive(subValue, subValue.GetType(), subItemExpression, browseProperties);
                }
            }
        }
        else
        {
            // Browse the properties of the objet
            foreach (PropertyInfo prop in sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {              
                Expression expression = Expression.Property(sourceExpression, prop.Name);
                object value = prop.GetValue(sourceObject);
                if (value is not null)
                {
                    AddObjectRecursive(value, value.GetType(), expression, browseProperties);
                }
            }
        }
    }

    /// <summary>
    /// Keep the expression corresponding to the instance of object.
    /// </summary>
    /// <param name="o">The instance to keep.</param>
    /// <param name="type">The type of the instance.</param>
    /// <param name="expression">The expression representing the object.</param>
    private void AddObject(object o, Type type, Expression expression)
    {
        if (!_objects.TryGetValue(type, out Dictionary<object, Expression> dico))
        {
            dico = new(ReferenceEqualityComparer.Default);
            _objects.Add(type, dico);
        }
        if (!dico.TryAdd(o, expression))
        {
            throw new Exception($"The same instance of {type.FullName} is used multiple times in the \"{nameof(LgGridView<object>.NewItem)}\" parameter result of the GridView.");
        }
    }

    #endregion

}
