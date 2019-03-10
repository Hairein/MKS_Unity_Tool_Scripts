// MKS Unity Level Exporter v1.2 (JSON)
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

public class MKS_UnityExportLevel_JSON : MonoBehaviour
{
    [UnityEditor.MenuItem("MKS Tools/JSON Level Exporter v1.2")]
    public static void LevelExporter()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(sceneName))
        {
            string filename = string.Format("{0}.json", sceneName);
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                string timeNow = DateTime.Now.ToString();

                streamWriter.WriteLine("{");

                WriteLinesToStream(streamWriter);

                streamWriter.WriteLine("}");

                streamWriter.Close();

                Debug.Log(string.Format("MKS Level Export JSON Complete [{0}].", timeNow));
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
                    outputLine.Append(BuildBody(index, currGameObject, listOfTransforms));

                    stream.Write(outputLine.ToString());

                }
            }

            if(index < listOfTransforms.Length - 1)
            {
                stream.WriteLine(",");
            }
            else
            {
                stream.WriteLine("");
            }
        }
    }

    private static string BuildBody(int index, GameObject currGameObject, Transform[] listOfTransforms)
    {
        StringBuilder outputLine = new StringBuilder();

        outputLine.Append(BuildIndex(index));
        outputLine.Append(":");
        outputLine.Append("{\n");

        outputLine.Append(BuildName(index, currGameObject));
        outputLine.Append(BuildTag(index, currGameObject));

        outputLine.Append(BuildCamera(index, currGameObject));

        outputLine.Append(BuildLight(index, currGameObject));

        outputLine.Append(BuildVisibility(index, currGameObject));
        outputLine.Append(BuildCollider(index, currGameObject));
        outputLine.Append(BuildParent(index, currGameObject, listOfTransforms));

        outputLine.Append(BuildLocalPosition(index, currGameObject));
        outputLine.Append(BuildLocalRotation(index, currGameObject));
        outputLine.Append(BuildLocalScale(index, currGameObject));

        outputLine.Append(BuildGlobalPosition(index, currGameObject));
        outputLine.Append(BuildGlobalRotation(index, currGameObject));
        outputLine.Append(BuildGlobalScale(index, currGameObject));

        outputLine.Append("\n}");

        return outputLine.ToString();
    }

    private static string BuildIndex(int index)
    {
        return string.Format("\"{0}\"", index.ToString(CultureInfo.InvariantCulture));
    }

    private static string BuildVisibility(int index, GameObject currGameObject)
    {
        string meshComponentStateText = "show";
        if (currGameObject.GetComponent<MeshRenderer>() != null && currGameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            meshComponentStateText = "hide";
        }

        return string.Format("\"visibility\":\"{0}\",\n", meshComponentStateText);
    }

    private static string BuildCollider(int index, GameObject currGameObject)
    {
        string meshColliderStateText = "no_collide";
        if (currGameObject.GetComponent<MeshCollider>() != null && currGameObject.GetComponent<MeshCollider>().enabled == true)
        {
            meshColliderStateText = "collide";
        }

        return string.Format("\"collider\":\"{0}\",\n", meshColliderStateText);
    }

    private static string BuildName(int index, GameObject currGameObject)
    {
        string name = currGameObject.name ?? "noname";

        return string.Format("\"name\":\"{0}\",\n", name);
    }

    private static string BuildTag(int index, GameObject currGameObject)
    {
        string tag = currGameObject.tag ?? "unknown";

        return string.Format("\"tag\":\"{0}\",\n", tag);
    }

    private static string BuildParent(int index, GameObject currGameObject, Transform[] listOfTransforms)
    {
        int parentIndex = -1;

        Transform parentTransform = currGameObject.transform.parent;
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

        return string.Format("\"parentIndex\":\"{0}\",\n", parentIndexText);
    }

    private static string BuildLocalPosition(int index, GameObject currGameObject)
    {
        Vector3 localPosition = currGameObject.transform.localPosition;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"localPosition\":{");

        outputLine.Append(string.Format("\"x\":{0},", localPosition[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", localPosition[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"z\":{0}", localPosition[2].ToString(CultureInfo.InvariantCulture)));

        outputLine.Append("},\n");

        return outputLine.ToString();
    }

    private static string BuildLocalRotation(int index, GameObject currGameObject)
    {
        Quaternion localQuaternion = currGameObject.transform.localRotation;
        Vector3 localRotation = localQuaternion.eulerAngles;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"localRotation\":{");

        outputLine.Append(string.Format("\"p\":{0},", localRotation[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", localRotation[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"r\":{0}", localRotation[2].ToString(CultureInfo.InvariantCulture)));

        outputLine.Append("},\n");

        return outputLine.ToString();
    }

    private static string BuildLocalScale(int index, GameObject currGameObject)
    {
        Vector3 localScale = currGameObject.transform.localScale;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"localScale\":{");

        outputLine.Append(string.Format("\"x\":{0},", localScale[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", localScale[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"z\":{0}", localScale[2]).ToString(CultureInfo.InvariantCulture));

        outputLine.Append("},\n");

        return outputLine.ToString();
    }

    private static string BuildGlobalPosition(int index, GameObject currGameObject)
    {
        Vector3 globalPosition = currGameObject.transform.position;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"globalPosition\":{");

        outputLine.Append(string.Format("\"x\":{0},", globalPosition[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", globalPosition[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"z\":{0}", globalPosition[2].ToString(CultureInfo.InvariantCulture)));

        outputLine.Append("},\n");

        return outputLine.ToString();
    }

    private static string BuildGlobalRotation(int index, GameObject currGameObject)
    {
        Quaternion globalQuaternion = currGameObject.transform.rotation;
        Vector3 globalRotation = globalQuaternion.eulerAngles;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"globalRotation\":{");

        outputLine.Append(string.Format("\"p\":{0},", globalRotation[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", globalRotation[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"r\":{0}", globalRotation[2].ToString(CultureInfo.InvariantCulture)));

        outputLine.Append("},\n");

        return outputLine.ToString();
    }

    private static string BuildGlobalScale(int index, GameObject currGameObject)
    {
        Vector3 globalScale = currGameObject.transform.lossyScale;

        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("\"globalScale\":{");

        outputLine.Append(string.Format("\"x\":{0},", globalScale[0].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"y\":{0},", globalScale[1].ToString(CultureInfo.InvariantCulture)));
        outputLine.Append(string.Format("\"z\":{0}", globalScale[2].ToString(CultureInfo.InvariantCulture)));

        outputLine.Append("}");

        return outputLine.ToString();
    }

    private static string BuildCamera(int index, GameObject currGameObject)
    {
        string result = string.Empty;

        Camera camera = currGameObject.GetComponent<Camera>();
        if (camera != null)
        {
            StringBuilder outputLine = new StringBuilder();
            outputLine.Append("\"camera\":{\n");

            outputLine.Append(string.Format("\"fieldOfView\":{0},\n", camera.fieldOfView.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"nearClipPlane\":{0},\n", camera.nearClipPlane.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"farClipPlane\":{0},\n", camera.farClipPlane.ToString(CultureInfo.InvariantCulture)));

            outputLine.Append(string.Format("\"pixelWidth\":{0},\n", camera.pixelWidth.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"pixelHeight\":{0},\n", camera.pixelHeight.ToString(CultureInfo.InvariantCulture)));

            string mode = camera.orthographic == true ? "orthographic" : "perspective";
            outputLine.Append(string.Format("\"mode\":\"{0}\"", mode));

            outputLine.Append("\n},\n");

            result = outputLine.ToString();
        }

        return result;
    }

    private static string BuildLight(int index, GameObject currGameObject)
    {
        string result = string.Empty;

        Light light = currGameObject.GetComponent<Light>();
        if (light != null)
        {
            StringBuilder outputLine = new StringBuilder();
            outputLine.Append("\"light\":{\n");

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
            outputLine.Append(string.Format("\"type\":\"{0}\",\n", lightTypeText));

            outputLine.Append("\"color\":{\n");
            outputLine.Append(string.Format("\"r\":{0},\n", light.color.r.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"g\":{0},\n", light.color.g.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"b\":{0},\n", light.color.b.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"a\":{0}", light.color.a.ToString(CultureInfo.InvariantCulture)));

            outputLine.Append("\n},\n");

            outputLine.Append(string.Format("\"intensity\":{0},\n", light.intensity.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"range\":{0},\n", light.range.ToString(CultureInfo.InvariantCulture)));
            outputLine.Append(string.Format("\"spotAngle\":{0}", light.spotAngle.ToString(CultureInfo.InvariantCulture)));

            outputLine.Append("\n},\n");

            result = outputLine.ToString();
        }

        return result;
    }
}

#endif
