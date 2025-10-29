namespace SubscriptionsService.Implementation;

public static class TestClass
{
	public static string Test ()
	{
		// Имплементация зависит от абстракции
		return nameof(Abstractions.TestClass.Test);
	}
}

