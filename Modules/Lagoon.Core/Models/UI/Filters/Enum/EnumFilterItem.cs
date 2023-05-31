using System.Runtime.CompilerServices;

namespace Lagoon.UI.Components;


/// <summary>
/// Enumeration filter expression.
/// </summary>
/// <typeparam name="TEnum">Enumeration type.</typeparam>
public sealed class EnumFilterItem<TEnum> : FilterItem<TEnum>
{
    ///<inheritdoc/>
    protected override IEnumerable<LambdaExpression> GetWhereIncludeExpressions(FilterWhereContext context)
    {
        //if (context)
        //{
        //    if (Values is not null && Values.Count > 0)
        //    {
        //        return new LambdaExpression[] { GetODataWhereInFunction() };
        //    }
        //    else
        //    {
        //        return Enumerable.Empty<LambdaExpression>();
        //    }
        //}
        //else
        //{
        return base.GetWhereIncludeExpressions(context);
        //}
    }

    //private LambdaExpression GetODataWhereInFunction()
    //{
    //    Expression where = null;
    //    Func<Expression, Expression, BinaryExpression> testFunction = Exclude ? Expression.NotEqual : Expression.Equal;
    //    Func<Expression, Expression, BinaryExpression> joinFunction = Exclude ? Expression.AndAlso : Expression.OrElse;
    //    ParameterExpression xParameter = Expression.Parameter(typeof(TEnum), "x");
    //    BinaryExpression binaryExpression;
    //    bool isNull = false;
    //    bool isNullable = Nullable.GetUnderlyingType(typeof(TEnum)) is not null;
    //    // Create the OR EQUAL or AND NOT EQUAL condition
    //    foreach (TEnum value in Values)
    //    {
    //        if (value is null)
    //        {
    //            isNull = true;
    //        }
    //        else
    //        {
    //            TEnum enumValue = value;
    //            object valueConverted = null;
    //            if (!isNullable)
    //            {
    //                valueConverted = Unsafe.As<TEnum, int>(ref enumValue);
    //            }
    //            else
    //            {
    //                valueConverted = Unsafe.As<TEnum, int?>(ref enumValue);
    //            }
    //            binaryExpression = testFunction(Expression.Convert(xParameter, typeof(int)), Expression.Constant(valueConverted, typeof(int)));
    //            where = where is null ? binaryExpression : joinFunction(where, binaryExpression);
    //        }
    //    }
    //    // If the type is a nullable of an enum
    //    if (isNullable)
    //    {
    //        // Conditional XOR to include or exclude null value
    //        if (isNull != !Exclude)
    //        {
    //            // Include null value
    //            testFunction = Expression.Equal;
    //            joinFunction = Expression.OrElse;
    //        }
    //        else
    //        {
    //            // Exclude null value
    //            testFunction = Expression.NotEqual;
    //            joinFunction = Expression.AndAlso;
    //        }
    //        // Test the null before other values
    //        binaryExpression = testFunction(xParameter, Expression.Constant(null, typeof(object)));
    //        where = where is null ? binaryExpression : joinFunction(binaryExpression, where);
    //    }
    //    return Expression.Lambda<Func<TEnum, bool>>(where, xParameter);
    //}
}
