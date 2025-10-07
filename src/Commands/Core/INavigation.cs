using System;

namespace Dotnet.Commands
{
	public interface INavigation
	{
		bool IsNavigationInProgress { get; }

		void NavigationCompleted(Type viewModelType);
	}
}
