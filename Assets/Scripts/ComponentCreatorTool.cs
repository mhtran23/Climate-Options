using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
using System;
using System.Linq;

public class ComponentCreatorTool : MonoBehaviour
{
    public TextBlock componentName;
    public TextBlock description;
    public TextBlock pros;
    public TextBlock cons;
    public TextBlock prerequisiteComponentType;
    public TextBlock isWholeHomeComponent;
    public TextBlock isHeating;
    public TextBlock isCooling;
    public TextBlock heatingBTUOutput;
    public TextBlock coolingBTUOutput;
    public TextBlock heatingCostPerBTU;
    public TextBlock coolingCostPerBTU;
    public TextBlock componentType;
    public TextBlock utilityType;
    public TextBlock priceRangeLow;
    public TextBlock priceRangeHigh;

    readonly List<TextBlock> textBlocks = new();

    public ClimateControlComponent component;
    public ClimateControlComponentFactory factory;

    private void Awake()
    {
        factory = GetComponent<ClimateControlComponentFactory>();

        textBlocks.Add(componentName);
        textBlocks.Add(description);
        textBlocks.Add(pros);
        textBlocks.Add(cons);
        textBlocks.Add(prerequisiteComponentType);
        textBlocks.Add(isWholeHomeComponent);
        textBlocks.Add(isHeating);
        textBlocks.Add(isCooling);
        textBlocks.Add(heatingBTUOutput);
        textBlocks.Add(coolingBTUOutput);
        textBlocks.Add(heatingCostPerBTU);
        textBlocks.Add(coolingCostPerBTU);
        textBlocks.Add(componentType);
        textBlocks.Add(utilityType);
        textBlocks.Add(priceRangeLow);
        textBlocks.Add(priceRangeHigh);
    }
    public void Reset()
    {
        foreach (var item in textBlocks)
        {
            item.Text = "";
        }
        component = null;
    }

    public void CreateClimateControlComponent()
    {

        string _componentName = componentName.Text;
        string _description = description.Text;
        string _pros = pros.Text;
        string _cons = cons.Text;
        List<ClimateControlComponentTypes> _prerequisiteComponentType = new();
        {
            string enumsString = prerequisiteComponentType.Text;
            string[] enumsArray = enumsString.Split(',');
            //_prerequisiteComponentType = enumsArray.Select(c => Enum.TryParse(c, out ClimateControlComponentTypes colorEnum) ? colorEnum : ClimateControlComponentTypes.Error).ToArray();
            foreach (var item in enumsArray)
            {
                if (item.CompareTo("") == 0) { continue; }
                if (Enum.TryParse(item, out ClimateControlComponentTypes _temp))
                {
                    _prerequisiteComponentType.Add(_temp);
                }
                else
                {
                    Debug.Log("Prerequisite Component Type Not Found");
                    return;
                }
            }

        }

        bool _isWholeHomeComponent = Convert.ToBoolean(isWholeHomeComponent.Text);
        bool _isHeating = Convert.ToBoolean(isHeating.Text);
        bool _isCooling = Convert.ToBoolean(isCooling.Text);
        float _heatingBTUOutput = float.Parse(heatingBTUOutput.Text);
        float _coolingBTUOutput = float.Parse(coolingBTUOutput.Text);
        float _heatingCostPerBTU = float.Parse(heatingCostPerBTU.Text);
        float _coolingCostPerBTU = float.Parse(coolingCostPerBTU.Text);
        if (!Enum.TryParse(componentType.Text, out ClimateControlComponentTypes _componentType))
        {
            Debug.Log("Component Type Not Found");
        }
        if (!Enum.TryParse(utilityType.Text, out UtilityType _utilityType))
        {
            Debug.Log("Utility Type Not Found");
        }
        (float, float) _priceRange = (float.Parse(priceRangeLow.Text), float.Parse(priceRangeHigh.Text));

        try
        {
            component = new(
                _componentName,
                _description,
                _pros,
                _cons,
                _prerequisiteComponentType,
                _isWholeHomeComponent,
                _isHeating,
                _isCooling,
                _heatingBTUOutput,
                _coolingBTUOutput,
                _heatingCostPerBTU,
                _coolingCostPerBTU,
                _componentType,
                _utilityType,
                _priceRange
                );
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Debug.Log("hit catch block");
        }
    }

    public void SendToJSON()
    {
        factory.SaveObjectToJsonFile(component, component.componentName);
    }
}