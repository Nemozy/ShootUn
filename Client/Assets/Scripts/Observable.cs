using System;
using System.Collections.Generic;

public class Observable<T> : IObservable<T>
{
    public static implicit operator T(Observable<T> f)
    {
        return f._value;
    }

    private T _value;
    public T Value => _value;

    public event Action<T> OnChange;

    public Observable()
    {
    }

    public Observable(T initialValue)
    {
        _value = initialValue;
    }

    internal void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value))
        {
            return;
        }

        _value = value;
        OnChange?.Invoke(_value);
    }
}