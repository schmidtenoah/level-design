using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "ScriptableObjects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public string _recipeName;

    [System.Serializable]
    public struct Ingredient
    {
        public CollectableItemSO _item;
        public int _quantity;
    }

    public Ingredient[] _ingredients;

    public CollectableItemSO _resultItem;
    
}
