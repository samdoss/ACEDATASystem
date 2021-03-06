using System;
using System.Data;
using System.Data.Common;
using ACE.PurchaseOrder.CommonLayer;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ACE.PurchaseOrder.DataLayer
{
  public class RelationshipDL : CommonForAllDL
  {
    #region Properties

    public int RelationshipID { get; set; }

    public string RelationshipDescription { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Get Relationship List
    /// </summary>
    /// <param name="ACEConnection">Instance of the ACEConnection Class</param>
    /// <returns>Return the data as Dataset</returns>
    public DataSet GetRelationshipList()
    {
      DataSet ds = new DataSet();
      try
      {
        Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
        string sqlCommand = "spGetRelationshipList";
        DbCommand dbCommand = db.GetStoredProcCommand(sqlCommand);
        ds = db.ExecuteDataSet(dbCommand);
      }
      catch (Exception ex)
      {
        ErrorLog.LogErrorMessageToDB("", "RelationshipDL.cs", "GetRelationshipList", ex.Message.ToString(), _myConnection);
      }
      return ds;
    }

    #endregion
  }
}
