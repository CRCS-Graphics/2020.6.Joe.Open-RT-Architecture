using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenRT {
    public class IntersectShaderCollectionGPUProgramGenerator : IShaderCollectionGPUProgramGenerator {

        public const string CUSTOMER_SHADER_COLLECTION_DIR = "Assets/SRP/ComputeShader/Intersect/";
        public const string CUSTOMER_SHADER_COLLECTION_FILENAME = "IntersectShaderCollection";

        public bool ExportShaderCollection(List<CustomShaderMeta> shadersImportMetaList) {
            return WriteToCustomShaderCollection(GenerateShaderCollectionFileContent(shadersImportMetaList));
        }

        public string GenerateShaderCollectionFileContent(List<CustomShaderMeta> shadersImportMetaList) {
            StringBuilder sb = new StringBuilder();
            // Order is reverse
            sb.AppendLine("// =============================================");
            sb.AppendLine("// =         Intersect Shader Collection       =");
            sb.AppendLine("// = Auto-generated File. Do not edit manually =");
            sb.AppendLine($"// = Time: {System.DateTime.Now.ToLongDateString()} {System.DateTime.Now.ToLongTimeString()} =");
            sb.AppendLine("// =============================================");
            sb.AppendLine();
            // sb.AppendLine("#pragma editor_sync_compilation");

            shadersImportMetaList.ForEach((shader) => {
                var relPath = shader.absPath.Replace(CUSTOMER_SHADER_COLLECTION_DIR, "");
                sb.AppendLine($"#include \"{relPath}\"");
            });

            sb.AppendLine("RayHit Trace(inout Ray ray, int instanceId)");
            sb.AppendLine("{");
            sb.AppendLine("    RayHit bestHit = CreateRayHit();");
            sb.AppendLine("    RayHit curHit;");
            for (int s = 0; s < shadersImportMetaList.Count; s++) {
                sb.AppendLine($"    curHit = {shadersImportMetaList[s].name}(ray);");
                sb.AppendLine($"    if (curHit.distance < bestHit.distance)");
                sb.AppendLine("    {");
                sb.AppendLine($"        bestHit = curHit;");
                sb.AppendLine("    }");
            }
            sb.AppendLine("    return bestHit;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public bool WriteToCustomShaderCollection(string content) {
            try {
                System.IO.File.WriteAllText(
                    CUSTOMER_SHADER_COLLECTION_DIR + CUSTOMER_SHADER_COLLECTION_FILENAME + ".compute",
                    content);
                return true;
            } catch (System.Exception ex) {
                Debug.LogException(ex);
                return false;
            }
        }
    }
}