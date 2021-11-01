using GameDevTV.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceField = null;
        Purse purse;

        private void Start() 
        {
            purse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();  
            purse.OnChange += RefreshUI;
            RefreshUI(); 
        }

        void RefreshUI()
        {
            balanceField.text = $"${purse.GetBalance():N2}";
        }
    }
}
