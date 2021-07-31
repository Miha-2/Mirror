using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Material", menuName = "Scriptable/Scriptable Material", order = 0)]
public class ScriptableMaterial : ScriptableObject
{
    public string Name;
    [Tooltip("-1 = unpenetratable, 0 = no density, (Real world material density, divided by 100)")] public float Density;
}