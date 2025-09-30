// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace ThinkSharp.FeatureTouring.ViewModels
{
  /// <summary>
  ///   The RelayCommand is a simple extensible implementation of the ICommand interface.
  /// </summary>
  public class RelayCommand : CommandBase
  {
    private readonly Predicate<Object>  m_myCanExecute;
    private readonly Action<Object> m_myExecute;


    // .ctor
    // ////////////////////////////////////////////////////////////////////////////////////////////

    #region .ctor

    /// <summary>
    /// Creates a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">
    ///   The delegate that will be executed.
    /// </param>
    public RelayCommand(Action<Object> execute)
      : this(execute, _ => true)
    {
    }

    /// <summary>
    /// Creates a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">
    ///   The delegate that will be executed.
    /// </param>
    /// <param name="canExecute">
    ///   The delegate that determines if the command can be executed.
    /// </param>
    public RelayCommand(Action<Object> execute, Predicate<Object> canExecute)
    {
        m_myExecute = execute ?? throw new ArgumentNullException(nameof(execute), @"execute can not be null.");

      m_myCanExecute = canExecute;
    }

    #endregion


    // Methods
    // ////////////////////////////////////////////////////////////////////////////////////////////

    #region CanExecute

    /// <summary>
    ///   Determines whether the command can execute in its current state. 
    /// </summary>
    /// <param name="parameter">
    ///   The parameter that contains optional information about the state.
    /// </param>
    /// <returns>
    ///   true if the command can be executed; otherwise false.
    /// </returns>
    public override Boolean CanExecute(Object parameter)
    {
        return m_myCanExecute(parameter);
    }

    #endregion


    #region Execute

    /// <summary>
    ///   Executes the command.
    /// </summary>
    /// <param name="parameter">
    ///   The parameter that contains optional data.
    /// </param>
    protected override void ExecuteInternal(Object parameter)
    {
      m_myExecute(parameter);
    }

    #endregion


    // Properties
    // ////////////////////////////////////////////////////////////////////////////////////////////


    // Events
    // ////////////////////////////////////////////////////////////////////////////////////////////

  }
}