using UnityEngine.Events;

public class ModelValue<ValueType>
{
    private ValueType value;
    private UnityEvent handleChange;

    public ModelValue(ValueType defaultValue)
    {
        handleChange = new UnityEvent();
        value = defaultValue;
    }

    public void AddListener(UnityAction unityAction)
    {
        handleChange.AddListener(unityAction);
    }

    public void RemoveListener(UnityAction unityAction)
    {
        handleChange.RemoveListener(unityAction);
    }

    public ValueType GetValue()
    {
        return value;
    }

    public void SetValue(ValueType newValue)
    {
        if (value.Equals(newValue))
        {
            return;
        }

        value = newValue;

        handleChange.Invoke();
    }

    public void OnDestroy()
    {
        handleChange.RemoveAllListeners();
    }
}