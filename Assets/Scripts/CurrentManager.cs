using UnityEngine;


public class CurrentManager : MonoBehaviour
{
    public static CurrentManager Instance { get; private set; }
    public int currentCoin = 100;

    public static event System.Action<int> OnCoinChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        { 
            Instance = this;
        }
    }

    public void AddCoin(int amount)
    {

        currentCoin += amount;
        Debug.Log("코인획득! 현재 코인 : " + currentCoin);

        OnCoinChanged?.Invoke(currentCoin);
    }

    public bool SubtractCoin(int amount)
    { 
        if (currentCoin >= amount)
        {
            currentCoin -= amount;
            Debug.Log("코인 사용! 현재 코인 : " + currentCoin);
            OnCoinChanged?.Invoke(currentCoin);
            return true;
        }
        Debug.Log("코인부족");
        return false;
    }
}