using UnityEngine;

namespace Game.Space
{
    #region//Required Components
    #endregion

    public class Template : MonoBehaviour
    {
        //Variable Initialization
        #region//Cached variables
        [Header("Example Heading")]
        [SerializeField] int serializedFirst;
        int nonSerializedSecond;
        #endregion
        #region//State variables
        [Header("Example Heading")]
        [SerializeField] int serializedFirstAgain;
        int nonSerializedSecondAgain;
        #endregion


        //Class Methods
        #region//Parent class methods
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region//First group of related Methods
        #endregion

        #region//Interface mehthods
        #endregion
    }

}