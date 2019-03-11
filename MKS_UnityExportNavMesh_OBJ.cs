// MKS Unity NavMesh Exporter v1.0 (JSON)
// by Micah Koleoso Software, 2019
// http://www.micahkoleoso.de
// For fixes, changes, feature requests, email contact@micahkoleoso.de


// Note:    Each area becomes a group, area ID becomes the group name 
//          No material or UV information is exported, only vertices and faces

#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.AI;
using System.Text;
using System.IO;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class MKS_UnityExportNavMesh_OBJ : MonoBehaviour
{
    [UnityEditor.MenuItem("MKS Tools/OBJ NavMesh Exporter v1.0")]
    public static void NavMeshExporter()
    {
        string sceneName = string.Format("{0}_NavMesh", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (!string.IsNullOrEmpty(sceneName))
        {
            string filename = string.Format("{0}.obj", sceneName);
            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                string timeNow = DateTime.Now.ToString();

                // Write object header
                StringBuilder outputLine = new StringBuilder();
                outputLine.Append("o ");
                outputLine.Append(sceneName);
                outputLine.Append("\n");
                streamWriter.Write(outputLine.ToString());

                WriteNavMeshGroups(streamWriter);

                streamWriter.Close();

                Debug.Log(string.Format("MKS NavMesh Export OBJ Complete [{0}].", timeNow));
            }
        }
    }

    private static void WriteNavMeshGroups(StreamWriter streamWriter)
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        if (triangulation.areas == null && triangulation.areas.Length == 0)
        {
            Debug.Log("Error: No NavMesh data to parse found.");
            return;
        }

        // Collect area ids in unique set
        HashSet<int> areaSet = new HashSet<int>();
        foreach(int areaId in triangulation.areas)
        {
            areaSet.Add(areaId);
        }
        if(areaSet.Count == 0)
        {
            Debug.Log("Error: No NavMesh area data to parse found.");
            return;
        }

        // Write vertices for all groups once
        WriteVertexDataforGroup(streamWriter, triangulation);

        // Write groups for each unique id
        foreach (int areaId in areaSet)
        {
            WriteDataForGroup(streamWriter, triangulation, areaId);
        }
    }

    private static void WriteDataForGroup(StreamWriter streamWriter, NavMeshTriangulation triangulation, int groupId)
    {
        streamWriter.Write("\n");

        // Write group header
        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("g ");
        outputLine.Append(groupId.ToString());
        outputLine.Append("\n");
        outputLine.Append("\n");

        streamWriter.Write(outputLine.ToString());

        WriteTriangleIndexDataforGroup(streamWriter, triangulation, groupId);
    }

    // NOTE: Entire vertex data is written out in each group to reuse indices
    // v X Y Z
    private static void WriteVertexDataforGroup(StreamWriter streamWriter, NavMeshTriangulation triangulation)
    {
        streamWriter.Write("\n");

        for (int vertexIndex = 0; vertexIndex < triangulation.vertices.Length; vertexIndex++)
        {
            Vector3 vertex = triangulation.vertices[vertexIndex];

            WriteVertexData(streamWriter, vertex.x, vertex.y, vertex.z);
        }
    }

    // f I J K
    private static void WriteTriangleIndexDataforGroup(StreamWriter streamWriter, NavMeshTriangulation triangulation, int groupId)
    {
        for(int areaIdIndex = 0; areaIdIndex < triangulation.areas.Length; areaIdIndex++)
        {
            int checkAreaId = triangulation.areas[areaIdIndex];
            if (checkAreaId != groupId)
            {
                continue;
            }

            int triangleIndex = areaIdIndex * 3;
            int index0 = triangulation.indices[triangleIndex];
            int index1 = triangulation.indices[triangleIndex + 1];
            int index2 = triangulation.indices[triangleIndex + 2];

            WriteFaceData(streamWriter, index0 + 1, index1 + 1, index2 + 1);
        }
    }

    // ---

    private static void WriteVertexData(StreamWriter streamWriter, float x, float y, float z)
    {
        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("v ");
        outputLine.Append(x.ToString(CultureInfo.InvariantCulture));
        outputLine.Append(" ");
        outputLine.Append(y.ToString(CultureInfo.InvariantCulture));
        outputLine.Append(" ");
        outputLine.Append(z.ToString(CultureInfo.InvariantCulture));
        outputLine.Append("\n");

        streamWriter.Write(outputLine.ToString());
    }

    private static void WriteFaceData(StreamWriter streamWriter, int i, int j, int k)
    {
        StringBuilder outputLine = new StringBuilder();
        outputLine.Append("f ");
        outputLine.Append(i.ToString());
        outputLine.Append(" ");
        outputLine.Append(j.ToString());
        outputLine.Append(" ");
        outputLine.Append(k.ToString());
        outputLine.Append("\n");

        streamWriter.Write(outputLine.ToString());
    }
}

#endif