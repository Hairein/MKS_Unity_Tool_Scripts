// MKS Unity Level Exporter v1.2 (TEXT)
// by Micah Koleoso Software, 2017
// http://www.micahkoleoso.de
// For fixes, changes, feature requests, email contact@micahkoleoso.de

// OUTPUT DATA:
// index,name,tag,
// OPTIONAL, IF HAS CAMERA COMPONENT [fieldOfView,nearClipPlane,farClipPlane,pixelWidth,pixelHeight,mode{orthographic|perspective},]
// OPTIONAL, IF HAS LIGHT COMPONENT [type{point,area,directional,spot},color{r,g,b,a},intensity,range,spotAngle,]
// visibility {show,hide},collider {no_collide|collide},parentIndex,
// localPosition,localRotation,localScale,
// globalPosition,globalRotation,globalScale

#if UNITY_EDITOR

using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Globalization;

public class MKS_UnityExportLevel_TEXT : MonoBehaviour
{
    [UnityEditor.MenuItem("MKS Tools/TEXT Level Exporter v1.2")]
    public static void LevelExporter()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(sceneName))
        {
            string filename = string.Format("{0}.txt", sceneName);
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                string timeNow = DateTime.Now.ToString();

                WriteLinesToStream(streamWriter);

                streamWriter.Close();

                Debug.Log(string.Format("MKS Level Export TEXT Complete [{0}].", timeNow));
            }
        }
    }

    private static void WriteLinesToStream(StreamWriter stream)
    {
        Transform[] listOfTransforms = FindObjectsOfType(typeof(Transform)) as Transform[];

        for (int index = 0; index < listOfTransforms.Length; index++)
        {
            Transform currTransform = listOfTransforms[index] as Transform;
            if (currTransform != null)
            {
                GameObject currGameObject = currTransform.gameObject;
                if (currGameObject != null)
                {
                    StringBuilder outputLine = new StringBuilder();

                    // Main
                    string indexText = index.ToString(CultureInfo.InvariantCulture);
                    outputLine.Append(indexText);
                    outputLine.Append(",");

                    string name = currGameObject.name ?? "noname";
                    outputLine.Append(name.Replace(",", string.Empty));
                    outputLine.Append(",");

                    string tag = currGameObject.tag ?? "unknown";
                    outputLine.Append(tag.Replace(",", string.Empty));
                    outputLine.Append(",");

                    // Camera
                    Camera camera = currGameObject.GetComponent<Camera>();
                    if (camera != null)
                    {
                        outputLine.Append(camera.fieldOfView.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(camera.nearClipPlane.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(camera.farClipPlane.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");

                        outputLine.Append(camera.pixelWidth.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(camera.pixelHeight.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");

                        string mode = camera.orthographic == true ? "orthographic" : "perspective";
                        outputLine.Append(mode);
                        outputLine.Append(",");
                    }

                    // Light
                    Light light = currGameObject.GetComponent<Light>();
                    if (light != null)
                    {
                        LightType lightType = light.type;
                        string lightTypeText = "point";
                        switch (lightType)
                        {
                            case LightType.Area:
                                lightTypeText = "area";
                                break;
                            case LightType.Directional:
                                lightTypeText = "directional";
                                break;
                            case LightType.Point:
                                lightTypeText = "point";
                                break;
                            case LightType.Spot:
                                lightTypeText = "spot";
                                break;
                            default:
                                break;
                        }
                        outputLine.Append(lightTypeText);
                        outputLine.Append(",");

                        outputLine.Append(light.color.r.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(light.color.g.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(light.color.b.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                        outputLine.Append(light.color.a.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");

                        float lightIntensity = light.intensity;
                        outputLine.Append(lightIntensity.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");

                        float lightRange = light.range;
                        outputLine.Append(lightRange.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");

                        float lightSpotAngle = light.spotAngle;
                        outputLine.Append(lightSpotAngle.ToString(CultureInfo.InvariantCulture));
                        outputLine.Append(",");
                    }

                    string meshComponentStateText = "show";
                    if (currGameObject.GetComponent<MeshRenderer>() != null && currGameObject.GetComponent<MeshRenderer>().enabled == false)
                    {
                        meshComponentStateText = "hide";
                    }
                    outputLine.Append(meshComponentStateText);
                    outputLine.Append(",");

                    string meshColliderStateText = "no_collide";
                    if (currGameObject.GetComponent<MeshCollider>() != null && currGameObject.GetComponent<MeshCollider>().enabled == true)
                    {
                        meshColliderStateText = "collide";
                    }
                    outputLine.Append(meshColliderStateText);
                    outputLine.Append(",");

                    // Parent
                    int parentIndex = -1;
                    Transform parentTransform = currTransform.parent;
                    if (parentTransform != null)
                    {
                        GameObject parentGameObject = parentTransform.gameObject;
                        if (parentGameObject != null)
                        {
                            for (int findParentIndex = 0; findParentIndex < listOfTransforms.Length; findParentIndex++)
                            {
                                if (findParentIndex == index)
                                    continue;

                                Transform searchTransform = listOfTransforms[findParentIndex] as Transform;
                                if (searchTransform != null)
                                {
                                    if (ReferenceEquals(parentTransform, searchTransform))
                                    {
                                        parentIndex = findParentIndex;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    string parentIndexText = parentIndex.ToString(CultureInfo.InvariantCulture);
                    outputLine.Append(parentIndexText);
                    outputLine.Append(",");

                    // Local position, rotation and scale
                    Vector3 localPosition = currTransform.localPosition;
                    outputLine.Append(localPosition[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localPosition[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localPosition[2].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");

                    Quaternion localQuaternion = currTransform.localRotation;
                    Vector3 localRotation = localQuaternion.eulerAngles;
                    outputLine.Append(localRotation[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localRotation[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localRotation[2].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");

                    Vector3 localScale = currTransform.localScale;
                    outputLine.Append(localScale[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localScale[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(localScale[2].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");

                    // Global position, rotation, scale
                    Vector3 globalPosition = currTransform.position;
                    outputLine.Append(globalPosition[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalPosition[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalPosition[2].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");

                    Quaternion globalQuaternion = currTransform.rotation;
                    Vector3 globalRotation = globalQuaternion.eulerAngles;
                    outputLine.Append(globalRotation[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalRotation[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalRotation[2].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");

                    Vector3 globalScale = currTransform.lossyScale;
                    outputLine.Append(globalScale[0].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalScale[1].ToString(CultureInfo.InvariantCulture));
                    outputLine.Append(",");
                    outputLine.Append(globalScale[2].ToString(CultureInfo.InvariantCulture));

                    stream.WriteLine(outputLine.ToString());
                }
            }
        }
    }
}

#endif
