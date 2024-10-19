using CoffeeHouse.Models;

namespace CoffeeHouse.Service
{
    public class DashboardService
    {
        private readonly ConnectDB _connectDB;

        public DashboardService()
        {
            _connectDB = new ConnectDB(); 
        }

  //      public async Task<int> GetToTalUser()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalUser = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (a:Account) RETURN count(a)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalUser;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();

  //          }

  //      }
  //      public async Task<float> getTotalOrder()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrder = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) RETURN SUM(o.TotalPrice) as totalAmount";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<float>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrder;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();

  //          }
  //      }
  //      public async Task<float> getTotalAmountToday()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalAmountToday = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) WHERE  date(datetime(o.Date)) = date() RETURN SUM(o.TotalPrice) as totalAmount";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<float>();
  //                  });
  //                  return total;
  //              });
  //              return totalAmountToday;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();

  //          }
  //      }
  //      public async Task<int> getOrderToday()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrderToday = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order)\r\nWHERE date(datetime(o.Date)) = date()\r\nRETURN count(o)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrderToday;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();
  //          }
  //      }
  //      public async Task<float> getTotalOrderMonth()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrderMonth = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) WHERE date(datetime(o.Date)) >= date() - duration('P1M') RETURN SUM(o.TotalPrice) as totalAmount";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<float>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrderMonth;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();
  //          }

  //      }
  //      public async Task<float> getTotalOrderYear()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrderYear = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) WHERE date(datetime(o.Date)) >= date() - duration('P1Y') RETURN SUM(o.TotalPrice) as totalAmount";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<float>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrderYear;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();
  //          }

  //      }
  //      public async Task<int> getTotalCategory()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalCate = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (c:Category) RETURN count(c)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalCate;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();

  //          }
  //      }
  //      public async Task<int> getTotalProduct()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalProduct = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (p:Product) RETURN count(p)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalProduct;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();

  //          }
  //      }
  //      public async Task<int> getOrderPending()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrderPending = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) WHERE o.Status = 'Pending' RETURN count(o)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrderPending;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();
  //          }
  //      }
  //      public async Task<int> getOrderCancel()
  //      {
  //          var session = _driver.AsyncSession();
  //          try
  //          {
  //              var totalOrderCancel = await session.ExecuteReadAsync(async transaction =>
  //              {
  //                  var readQuery = "MATCH (o:Order) WHERE o.Status = 'Cancel' RETURN count(o)";
  //                  var readResult = await transaction.RunAsync(readQuery);
  //                  var total = await readResult.SingleAsync(record =>
  //                  {
  //                      return record[0].As<int>();
  //                  });
  //                  return total;
  //              });
  //              return totalOrderCancel;
  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //          finally
  //          {
  //              await session.CloseAsync();
  //          }
  //      }
  //      public async Task<List<Orders>> GetOrders()
  //      {
  //          var session = _driver.AsyncSession();

  //          var orders = await session.ExecuteReadAsync(async transaction =>
  //          {
  //              var readQuery = "MATCH (o:Order) RETURN o";
  //              var readResult = await transaction.RunAsync(readQuery);
  //              var order = await readResult.ToListAsync(record =>
  //              {
  //                  var order = record["o"].As<INode>().Properties;
  //                  return new Orders
  //                  {
  //                      Id = order["Id"].As<int>(),
  //                      Date = order["Date"].As<DateTime>(),
  //                      TotalPrice = order["TotalPrice"].As<float>(),
  //                      Status = order["Status"].As<string>(),
  //                      A_Id = order["A_Id"].As<int>()
  //                  };
  //              });
  //              return order;
  //          });

  //          await session.CloseAsync();
  //          return orders;



  //      }
  //      public async Task<List<Orders>> GetAllOrderPending()
  //      {
  //          var session = _driver.AsyncSession();



		//	var orders = await session.ExecuteReadAsync(async transaction =>
		//	{
		//		var readQuery = "MATCH (o:Order) WHERE o.Status = 'Pending' RETURN o";
		//		var readResult = await transaction.RunAsync(readQuery);
		//		var order = await readResult.ToListAsync(record =>
		//		{
		//			var order = record["o"].As<INode>().Properties;
		//			return new Orders
		//			{
		//				Id = order["Id"].As<int>(),
		//				Date = order["Date"].As<DateTime>(),
		//				TotalPrice = order["TotalPrice"].As<float>(),
		//				Status = order["Status"].As<string>(),
		//				A_Id = order["A_Id"].As<int>()
		//			};
		//		});
		//		return order;
		//	});
		//	await session.CloseAsync();
		//	return orders;

  //      }
  //      public async Task<List<Orders>> GetAllOrderCancel()
  //      {
  //          var session = _driver.AsyncSession();

  //          var orders = await session.ExecuteReadAsync(async transaction =>
  //          {
  //              var readQuery = "MATCH (o:Order) WHERE o.Status = 'Cancel' RETURN o";
  //              var readResult = await transaction.RunAsync(readQuery);
  //              var order = await readResult.ToListAsync(record =>
  //              {
  //                  var order = record["o"].As<INode>().Properties;
  //                  return new Orders
  //                  {
  //                      Id = order["Id"].As<int>(),
  //                      Date = order["Date"].As<DateTime>(),
  //                      TotalPrice = order["TotalPrice"].As<float>(),
  //                      Status = order["Status"].As<string>(),
  //                      A_Id = order["A_Id"].As<int>()
  //                  };
  //              });
  //              return order;
  //          });
  //          await session.CloseAsync();
  //          return orders;
  //      }


  //      public async Task<List<Orders>> GetAllOrderDeli()
  //      {
  //          var session = _driver.AsyncSession();
  //          var orders = await session.ExecuteReadAsync(async transaction =>
  //          {
  //              var readQuery = "MATCH (o:Order) WHERE o.Status = 'Cancel' RETURN o";
  //              var readResult = await transaction.RunAsync(readQuery);
  //              var order = await readResult.ToListAsync(record =>
  //              {
  //                  var order = record["o"].As<INode>().Properties;
  //                  return new Orders
  //                  {
  //                      Id = order["Id"].As<int>(),
  //                      Date = order["Date"].As<DateTime>(),
  //                      TotalPrice = order["TotalPrice"].As<float>(),
  //                      Status = order["Status"].As<string>(),
  //                      A_Id = order["A_Id"].As<int>()
  //                  };
  //              });
  //              return order;
  //          });
  //          await session.CloseAsync();
  //          return orders;

  //      }

   
		//public async Task<Orders> GetOrderAsyncById(int id)
		//{
		//	var session = _driver.AsyncSession();

		//	var orders = await session.ExecuteReadAsync(async transaction =>
		//	{
		//		var readQuery = "MATCH (p:Order) WHERE p.Id = $id RETURN p";
		//		var readResult = await transaction.RunAsync(readQuery, new { id });
		//		var order = await readResult.SingleAsync(record =>
		//		{
		//			var product = record["p"].As<INode>().Properties;
		//			return new Orders
		//			{
		//				Id = product["Id"].As<int>(),
		//				Status = product["Status"].As<string>(),
		//			};
		//		});
		//		return order;
		//	});
		//	await session.CloseAsync();
		//	return orders;
		//}
		//public async Task<Orders> ChangeOrderStatus(int id,string status)
		//{
		//	var session = _driver.AsyncSession();
		//	try
		//	{
		//		var order = await session.ExecuteWriteAsync(async transaction =>
		//		{
		//			var writeQuery = "MATCH (o:Order) WHERE o.Id = $id SET o.Status = $status RETURN o";
		//			var writeResult = await transaction.RunAsync(writeQuery, new { id, status });
		//			var order = await writeResult.SingleAsync(record =>
		//			{
		//				var order = record["o"].As<INode>().Properties;
		//				return new Orders
		//				{
		//					Id = order["Id"].As<int>(),
		//					Status = order["Status"].As<string>(),
		//				};
		//			});
		//			return order;
		//		});
		//		return order;
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}
		//	finally
		//	{
		//		await session.CloseAsync();
		//	}
		//}
	}
}