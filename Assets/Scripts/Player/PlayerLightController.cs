using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    private Light2D light2D;

    void Start()
    {
        // 通过层级路径找到 Light 2D 对象
        GameObject light2DObject = GameObject.Find("Main Camera/Light 2D");
        if (light2DObject != null)
        {
            // 获取 Light2D 组件
            light2D = light2DObject.GetComponent<Light2D>();

            if (light2D != null)
            {
                LightManager.RegisterLight(light2D);
            }
            else
            {
                Debug.LogError("Light2D component not found on Light 2D GameObject.");
            }
        }
        else
        {
            Debug.LogError("Light 2D GameObject not found.");
        }
    }
}

public static class LightManager
{
    private static Light2D light2D;

    public static void RegisterLight(Light2D light)
    {
        light2D = light;
        //Debug.Log("Light2D registered.");
    }

    public static void AddFalloff(int addFalloff)
    {
        if (light2D != null)
        {
            light2D.shapeLightFalloffSize += addFalloff;
            Debug.Log("Falloff added: " + addFalloff);
        }
        else
        {
            Debug.LogError("Light2D is not registered.");
        }
    }

    public static bool CheckForWin()
    {
        if (light2D != null)
        {
            if (light2D.shapeLightFalloffSize >= 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
