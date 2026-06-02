using UnityEngine;
using System.Collections.Generic;


namespace DS.Data.Error
{
    using Elements;

    public class DSNodeErrorData : MonoBehaviour
    {
        public DSErrorData ErrorData {  get; set; }

        public List<DSNode> Nodes { get; set; }

        public DSNodeErrorData()
        {
            ErrorData = new DSErrorData();
            Nodes = new List<DSNode>();
        }


    }
}

