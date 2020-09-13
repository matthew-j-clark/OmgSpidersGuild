using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

using SharedModels;

using StorageUtilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SpidersGoogleSheetsIntegration
{

    /* TODO: 
     * 1. Locking on signup
     * 2. fix up some 
     * 3. 
     * 
     */
    public class SheetsClient
    {
        private static SheetsService SheetsServiceSingleton;

        public SheetsService SheetsService => SheetsServiceSingleton;
        public static string HeroicSheetsId { get; private set; }
        
        public SheetsClient()
        {
           
        }

        public async Task Initialize()
        {
            if(this.SheetsService!=null)
            {
                return;
            }

            var kvClient = new BasicKeyVaultClient();
            var googleBotClientId = await kvClient.GetSecretAsync("GoogleApiBotClientId");
            var googleBotPrivateKey = await kvClient.GetSecretAsync("GoogleApiPrivateKey");
            googleBotPrivateKey = googleBotPrivateKey.Replace("\\n", "\n");
            HeroicSheetsId = await kvClient.GetSecretAsync("HeroicSignupsSheetId");            

            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(googleBotClientId)
            {
                Scopes = new[] { SheetsService.Scope.Spreadsheets }
            }.FromPrivateKey(googleBotPrivateKey));

            SheetsServiceSingleton = new SheetsService(
                new BaseClientService.Initializer() 
                { 
                    HttpClientInitializer = credential, 
                    ApplicationName = "OMG Spiders Bot"
                });
           
        }

        public async Task RevokeSignupAsync(string name, string mainName)
        {
            var sheetToUpdate = await GetHeroicSignupSheet();
            var rowData = GetHeroicSignupRows(sheetToUpdate);
            RowData rowToUpdate = null;
            var rowIndex = FindSingleSignupBasedOnCharacterName(rowData, name, ref rowToUpdate);


            UpdateSignupRow(string.Empty, false, false, false, CharacterClass.None, false, false, string.Empty, rowToUpdate);
            await CommitSignupRow(sheetToUpdate, rowToUpdate, rowIndex);

        }

        public async Task<string> UpdateSignupAsync(string name,
            bool isTank,
            bool isHealer,
            bool isDps,
            CharacterClass characterClass,
            bool willFunnel,
            bool badalt,
            string mainName)
        {
            var sheetToUpdate = await GetHeroicSignupSheet();
            var rowData = GetHeroicSignupRows(sheetToUpdate);
            RowData rowToUpdate = null;

            var rowIndex = FindSingleSignupBasedOnCharacterName(rowData, name, ref rowToUpdate);

            if(rowToUpdate.Values[SignupRow.MainName].UserEnteredValue.StringValue.Equals(mainName, StringComparison.OrdinalIgnoreCase))
            {
                await RevokeSignupAsync(name, mainName);
                await AddSignupAsync(name, isTank, isHealer, isDps, characterClass, willFunnel, badalt, mainName);
                return $"Successfully updated the signup for: {name} with mainname {mainName}";
            }
            else
            {
                return $"{name} does not belong to you. It belongs to {rowToUpdate.Values[9]}";
            }
        }

        public async Task AddSignupAsync(
            string name, 
            bool isTank, 
            bool isHealer, 
            bool isDps, 
            CharacterClass characterClass,
            bool willFunnel,
            bool badalt, 
            string mainName)
        {            
            var sheetToUpdate = await GetHeroicSignupSheet();
            var rowData = GetHeroicSignupRows(sheetToUpdate);
            RowData rowToUpdate = null;

           var existingSignupRow = FindSingleSignupBasedOnCharacterName(rowData, name, ref rowToUpdate);
           if(existingSignupRow!=-1)
           {
               throw new SheetsSignupException($"{name} is already signed up by @{rowData[existingSignupRow].Values[SignupRow.MainName].UserEnteredValue.StringValue}");
           }
           //
            var rowIndex = FindSingleSignupBasedOnCharacterName(rowData, null, ref rowToUpdate);
            UpdateSignupRow(name, isTank, isHealer, isDps, characterClass, willFunnel, badalt, mainName, rowToUpdate);


            await CommitSignupRow(sheetToUpdate, rowToUpdate, rowIndex);

        }

        private void UpdateSignupRow(string name, bool isTank, bool isHealer, bool isDps, CharacterClass characterClass, bool willFunnel, bool badalt, string mainName, RowData rowToUpdate)
        {
            SetupHeroicSaleRowData(rowToUpdate);
            rowToUpdate.Values[SignupRow.Name].UserEnteredValue = new ExtendedValue() { StringValue = name };
            rowToUpdate.Values[SignupRow.Tank].UserEnteredValue = new ExtendedValue() { BoolValue = isTank };
            rowToUpdate.Values[SignupRow.Heal].UserEnteredValue = new ExtendedValue() { BoolValue = isHealer };
            rowToUpdate.Values[SignupRow.DPS].UserEnteredValue = new ExtendedValue() { BoolValue = isDps };
            rowToUpdate.Values[SignupRow.Class].UserEnteredValue = new ExtendedValue() { StringValue = characterClass== CharacterClass.None? string.Empty:characterClass.ToString() };
            rowToUpdate.Values[SignupRow.WillFunnel].UserEnteredValue = new ExtendedValue() { BoolValue = willFunnel };
            rowToUpdate.Values[SignupRow.BadAlt].UserEnteredValue = new ExtendedValue() { BoolValue = badalt };
            rowToUpdate.Values[SignupRow.MainName].UserEnteredValue = new ExtendedValue() { StringValue = mainName };
            rowToUpdate.Values[SignupRow.Date].UserEnteredValue = new ExtendedValue() { StringValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        }

        private async Task CommitSignupRow(Sheet sheetToUpdate, RowData rowToUpdate, int rowIndex)
        {
            var sheetUpdates = new BatchUpdateSpreadsheetRequest();
            sheetUpdates.Requests = new List<Request>() {
                new Request() {
                    UpdateCells = new UpdateCellsRequest()
                    {
                        Start=new GridCoordinate(){ SheetId=sheetToUpdate.Properties.SheetId, RowIndex=rowIndex, ColumnIndex=0 },
                         Fields= "*" ,
                        Rows = new List<RowData>() { rowToUpdate }
                    }
                }
            };

            var updateRequest = new SpreadsheetsResource.BatchUpdateRequest(this.SheetsService, sheetUpdates, HeroicSheetsId);

            await updateRequest.ExecuteAsync();
        }

        private static int FindSingleSignupBasedOnCharacterName(IList<RowData> rowData, string targetValue, ref RowData rowToUpdate)
        {
            int rowIndex;
            bool foundName = false;
            for (rowIndex = 0; rowIndex < rowData.Count&&!foundName; ++rowIndex)
            {
                var nameDataValue = rowData[rowIndex].Values[SignupRow.Name].UserEnteredValue;
                if(targetValue != null && nameDataValue == null)
                {
                    continue;
                }

                if ((targetValue == null && nameDataValue == null)
                    ||
                    nameDataValue.StringValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase))
                {                
                    rowToUpdate = rowData[rowIndex];
                    foundName = true;
                }
            }

            if(!foundName)
            {
                rowIndex = -1;
            }

            return rowIndex;
        }

        private async Task<Sheet> GetHeroicSignupSheet()
        {
            var range = "'Signup Sheet'!A1:K86";

            var sheetRequest = new SpreadsheetsResource.GetRequest(this.SheetsService, HeroicSheetsId);
            sheetRequest.Ranges = new Google.Apis.Util.Repeatable<string>(new[] { range });
            sheetRequest.IncludeGridData = true;

            var sheet = await sheetRequest.ExecuteAsync();
            var sheetToUpdate = sheet.Sheets[0];
            var rowData = sheetToUpdate.Data.First().RowData;
            return sheetToUpdate;
        }

        private IList<RowData> GetHeroicSignupRows(Sheet sheet)
        {
            return sheet.Data.First().RowData;
        }

        public void SetupHeroicSaleRowData(RowData row)
        {
            var booleanDvRule    = new DataValidationRule() { Condition = new BooleanCondition() { Type = "BOOLEAN" } };
            while (row.Values.Count < 11)
            {
                row.Values.Add(new CellData());
            }          
        }
    }
}
