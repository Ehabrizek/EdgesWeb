using System.Data;
using System.Data.OleDb;

namespace EdgesWebAPI.Logic
{
    public class CombinationProcessor
    {
        #region Public Methods
        
        public string WriteCombinations(string fullPathToExcel, ExcelFileInfo fileInfo)
        {
            string conn = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fullPathToExcel};Extended Properties='Excel 12.0;HDR=yes'";

            DataTable pathwaysTable = GetDataTable($"SELECT [{fileInfo.pathwayHeader}] FROM [{fileInfo.sheetName}$];", conn);
            DataTable distinctPathwaysTable = GetDataTable($"SELECT distinct [{fileInfo.pathwayHeader}] FROM [{fileInfo.sheetName}$] ORDER BY [{fileInfo.pathwayHeader}];", conn);
            DataTable geneIdsTable = GetDataTable($"SELECT [{fileInfo.geneHeader}] FROM [{fileInfo.sheetName}$];", conn);
            DataTable distinctKeggIdTable = GetDataTable($"SELECT distinct [{fileInfo.keggId}], [{fileInfo.pathwayHeader}] from [{fileInfo.sheetName}$] ORDER BY [{fileInfo.pathwayHeader}] ;", conn);

            List<List<string>> listOLists = new List<List<string>>();

            List<string> pathways = convertToStringList(pathwaysTable.Rows);
            List<string> distinctPathways = convertToStringList(distinctPathwaysTable.Rows);
            List<string> geneIds = convertToStringList(geneIdsTable.Rows);
            List<string> distinctKeggIds = convertToStringList(distinctKeggIdTable.Rows);

            foreach (string distinctPathway in distinctPathways)
            {
                //Create 1 list per grouping of the same pathways.
                List<string> combination = new List<string>();
                for (int i = 0; i < geneIds.Count; i++)
                {
                    if (pathways[i] == distinctPathway)
                    {
                        combination.Add(geneIds[i]);
                    }
                }
                listOLists.Add(combination);
            }
            string guid = Guid.NewGuid().ToString();
            string fileName = $"{guid}.csv";
            using (StreamWriter writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "files", fileName), true))
            {
                for (int i = 0; i < listOLists.Count; i++)
                {
                    string[] arr = listOLists[i].ToArray();
                    long r = 2;
                    long n = arr.Length;
                    prlongCombination(arr, n, r, distinctPathways[i], distinctKeggIds[i], writer);
                }
            }
            return $"./files/{fileName}";
        }

        #endregion

        #region Private Methods

        void prlongCombination(string[] arr, long n, long r, string pathWayName, string keggId, StreamWriter writer)
        {
            string[] data = new string[r];

            combinationUtilRecursive(arr, data, 0, n - 1, 0, r, pathWayName, keggId, writer);
        }

        void combinationUtilRecursive(string[] arr, string[] data, long start, long end, long index, long r, string pathWayName, string keggId, StreamWriter writer)
        {
            if (index == r)
            {
                for (long j = 0; j < r; j++)
                {
                    writer.Write(data[j] + "," + " ");
                }
                writer.Write(pathWayName + "," + " ");
                writer.Write(keggId);
                writer.WriteLine("");
                return;
            }

            for (long i = start; i <= end && end - i + 1 >= r - index; i++)
            {
                data[index] = arr[i];
                combinationUtilRecursive(arr, data, i + 1, end, index + 1, r, pathWayName, keggId, writer);
            }
        }

        DataTable GetDataTable(string sql, string connectionString)
        {
            DataTable dt = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    using (OleDbDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }

        List<string> convertToStringList(DataRowCollection dataRows)
        {
            List<string> stringList = new List<string>();
            foreach (DataRow row in dataRows)
            {
                stringList.Add(row.ItemArray[0].ToString());
            }
            return stringList;
        }

        #endregion
    }
}