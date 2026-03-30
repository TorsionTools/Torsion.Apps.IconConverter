using System.Windows.Input;

namespace Torsion.Apps.IconConverter.Utils;

internal class RelayCommand : ICommand
{
    private Action MainAction;
    /// <summary>
    /// The event thats fired when the <see cref="CanExecute(object)"/>	value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged = (sender, e) => { };

    public RelayCommand(Action action)
    {
        MainAction = action;
    }

    #region Command Methods
    /// <summary>
    /// Relay command can always execute
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        MainAction();
    }
    #endregion
}
