using UnityEngine;


namespace DS.Data.Error
{
    public class DSErrorData
    {
        public Color color{get; set;}

        private void GenerateRandomColor()
        {
            color = new Color32(
                Random.Range(65, 255),
                Random.Range(50, 175),
                Random.Range(50, 175)
                );
        }
    }
}

