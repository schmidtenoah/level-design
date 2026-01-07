using UnityEngine;

/*
 * https://assetstore.unity.com/packages/templates/tutorials/unity-learn-creator-kit-fps-urp-149310
 */

public class LiquidAmmoDisplay : MonoBehaviour
{
    
    [SerializeField] private LiquidAmmoContainer _container;
    [SerializeField] private float _minLiquidAmount;
    [SerializeField] private float _maxLiquidAmount;
    
    /**
     * Verändert den Flüssigkeitsstand anhand der beschränkungen dieser Anzeige.
     */
    public void UpdateAmount(int current, int max)
    { 
        _container.ChangeLiquidAmount(Mathf.Lerp(_minLiquidAmount, _maxLiquidAmount, current/(float)max));
    }
    
}
