using CoffeeHouse.Models;
using CoffeeHouse.Service;

namespace CoffeeHouse.Service
{
	public class AccountService
	{
        private readonly ConnectDB _connectDB;

        public AccountService()
        {
            _connectDB = new ConnectDB();  
        }


  //      public async Task<List<Accounts>> getAllAccountAsync()
		//{
		//	var session = _driver.AsyncSession();

		//	var result = await session.ExecuteReadAsync(async transaction =>
		//	{
		//		var readQuery = "MATCH (a:Account) RETURN a";
		//		var readResult = await transaction.RunAsync(readQuery);
		//		var accounts = await readResult.ToListAsync(record =>
		//		{
		//			var node = record["a"].As<INode>();
		//			return new Accounts
		//			{
		//				A_ID = node.Properties["Id"].As<int>(),
		//				A_NAME = node.Properties["Name"].As<string>(),
		//				A_EMAIL = node.Properties["Email"].As<string>(),
		//				A_PASSWORD = node.Properties["Password"].As<string>(),
		//				ROLE_ID = node.Properties["Role_Id"].As<int>(),
		//			};
		//		});
		//		return accounts;
		//	});
		//	await session.CloseAsync();
		//	return result;
		//}

	}

}

