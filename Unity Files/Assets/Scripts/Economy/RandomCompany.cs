using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static BaseCharacterIntelligence;

public class RandomCompany : MonoBehaviour
{
     public static RandomCompany instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void CreateRandomProduct()
    {
        Product product = new Product();
        GetProductInformation(product);
        CreateJob.instance.AddProduct(this, product);
    }

    private void GetProductInformation(Product product)
    {
        string randomName = Random.Range(0, 1000).ToString();
        product.Name = randomName;
        product.Company = GenerateName();
        product.Type = JobType.Game.ToString();
        product.Language = "C#";
        product.Complexity = 3;
        product.Price = Random.Range(1, 100);

        product.Age = 0;

        int quality = product.Complexity * (product.Complexity * Random.Range(1, 10));
        product.Quality = quality;

        int popularityModifier = product.Complexity * quality;
        product.Popularity = popularityModifier;
    }

    public class NameGen
    {
        public List<string> company_names = new List<string>();
    }

    public string GenerateName()
    {
        TextAsset namesJson = new TextAsset("Hello");
        namesJson = GameVariableConnector.instance.GetCompanyNames();

        if(namesJson == null)
        {
            Debug.LogError("No names.json file found!");
            return "NULL";
        }

        var classNames = JsonUtility.FromJson<NameGen>(namesJson.text);
        var name = classNames.company_names[Random.Range(0, classNames.company_names.Count)];
        return name;
    }

    private enum JobType
    {
        Game,
        Software,
        Web,
        Security
    }
}
