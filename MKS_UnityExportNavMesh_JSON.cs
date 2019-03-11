// MKS Unity NavMesh Exporter v1.0 (JSON)
// by Micah Koleoso Software, 2019
// http://www.micahkoleoso.de
// For fixes, changes, feature requests, email contact@micahkoleoso.de

// NOTE:    No material or UV information is exported, only area IDs, vertices and faces

#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Text;
using System.IO;
using System;
using System.Globalization;

public class MKS_UnityExportNavMesh_JSON : MonoBehaviour
{
    [UnityEditor.MenuItem("MKS Tools/JSON NavMesh Exporter v1.0")]
    public static void NavMeshExporter()
    {
        string sceneName = string.Format("{0}_NavMesh", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        if (!string.IsNullOrEmpty(sceneName))
        {
            string filename = string.Format("{0}.json", sceneName);
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                string timeNow = DateTime.Now.ToString();

                streamWriter.WriteLine("{");

                WriteNavMeshLines(streamWriter);

                streamWriter.WriteLine("}");

                streamWriter.Close();

                Debug.Log(string.Format("MKS NavMesh Export JSON Complete [{0}].", timeNow));
            }
        }
    }

    private static void WriteNavMeshLines(StreamWriter streamWriter) 
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation(); 
        
        // int[] areas
         if(triangulation.areas != null && triangulation.areas.Length > 0)
        {
            StringBuilder outputLine = new StringBuilder();

            for(int index = 0; index < triangulation.areas.Length; index++)
            {
                if(outputLine.Length > 0) 
                {
                    outputLine.Append(",");
                }

                outputLine.Append(triangulation.areas[index].ToString());
            }

            string finalLine = string.Format("\"areas\":[{0}],\n", outputLine.ToString());
            streamWriter.Write(finalLine);
        }
        
        // int[] indices
        if(triangulation.indices != null && triangulation.indices.Length > 0)
        {
            StringBuilder outputLine = new StringBuilder();

            for(int index = 0; index < triangulation.indices.Length; index++)
            {
                if(outputLine.Length > 0) 
                {
                    outputLine.Append(",");
                }

                outputLine.Append(triangulation.indices[index].ToString());
            }

            string finalLine = string.Format("\"indices\":[{0}],\n", outputLine.ToString());
            streamWriter.Write(finalLine);
        }
        
        // Vector3[] vertices
        if(triangulation.vertices != null && triangulation.vertices.Length > 0)
        {
            StringBuilder outputLine = new StringBuilder();

            for(int index = 0; index < triangulation.vertices.Length; index++)
            {
                if(outputLine.Length > 0) 
                {
                    outputLine.Append(",");
                }

                Vector3 vertex = triangulation.vertices[index];
                string vertexLine = string.Format("[{0},{1},{2}]", vertex.x.ToString(CultureInfo.InvariantCulture), vertex.y.ToString(CultureInfo.InvariantCulture), vertex.z.ToString(CultureInfo.InvariantCulture));
                outputLine.Append(vertexLine);
            }

            string finalLine = string.Format("\"vertices\":[{0}]\n", outputLine.ToString());
            streamWriter.Write(finalLine);
        }
    }
}

#endif