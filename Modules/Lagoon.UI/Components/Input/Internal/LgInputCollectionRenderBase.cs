namespace Lagoon.UI.Components.Input.Internal;

/// <summary>
/// Input render base with collection value
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class LgInputCollectionRenderBase<TValue> : LgInputRenderBase<ICollection<TValue>>
{
    ///<inheritdoc/>
    protected override void SavePreviousValue(ICollection<TValue> value)
    {
        if (value is null)
        {
            PreviousValue = null;
        }
        else
        {
            PreviousValue = CopyCollection(value);
        }
    }

    ///<inheritdoc/>
    public override Task CancelValueActionAsync()
    {
        CurrentValue = CopyCollection(PreviousValue);
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Return an copy of the collection
    /// </summary>
    /// <param name="collection">The collection to copy.</param>
    /// <returns></returns>
    private static ICollection<TValue> CopyCollection(ICollection<TValue> collection)
    {
        if (collection is null)
        {
            return null;
        }
        return (ICollection<TValue>)Activator.CreateInstance(collection.GetType(), collection);
    }
}
