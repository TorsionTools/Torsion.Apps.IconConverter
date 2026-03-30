using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Torsion.Apps.IconConverter.Models;

namespace Torsion.Apps.IconConverter.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    #region Properties
    internal Window Win { get; set; }
    public string WindowTitle { get; set; }
    public bool CanCheck { get; set; } = true;
    public string Icon { get; set; } = $"pack://application:,,,/{AppProps.AssemblyName};component/Images/TorsionTools.ico";
    #endregion

    public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    #region Command Helpers
    protected static async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
    {
        if(updatingFlag.GetPropertyValue())
        {
            return;
        }

        updatingFlag.SetPropertyValue(true);

        try
        {
            await action();
        }
        finally
        {
            updatingFlag.SetPropertyValue(false);
        }
    }
    #endregion

}
internal static class BaseModelHelpers
{
    public static T GetPropertyValue<T>(this Expression<Func<T>> lambda)
    {
        return lambda.Compile().Invoke();
    }

    public static void SetPropertyValue<T>(this Expression<Func<T>> lambda, T value)
    {
        MemberExpression expression = (lambda as LambdaExpression).Body as MemberExpression;
        PropertyInfo propertyInfo = (PropertyInfo)expression.Member;
        object target = System.Linq.Expressions.Expression.Lambda(expression.Expression).Compile().DynamicInvoke();
        propertyInfo.SetValue(target, value);
    }
}
