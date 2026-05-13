using System;
using System.Windows.Input;
// ReSharper disable All

namespace MepManholeTool.Commands
{
    public class ManholeToolCommand : ICommand
    {
        /// <summary>
        /// コマンドが実行可能かどうか示す値を返す関数を設定します。
        /// </summary>
        private Func<bool>? canExecuteHandler = null;

        /// <summary>
        /// コマンドの実行メソッドを設定します。
        /// </summary>
        private Action? executeHandler = null;

        /// <summary>
        /// コマンドが実行可能かどうか示す値を返す関数を設定します。
        /// </summary>
        private System.Func<object, bool>? canExecuteParaHandler = null;

        /// <summary>
        /// コマンドの実行メソッドを設定します。
        /// </summary>
        private System.Action<object>? executeParaHandler = null;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public ManholeToolCommand()
        {
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="canExecuteHandler">コマンドが実行可能かどうか示す値を返す関数</param>
        /// <param name="executeHandler">コマンドの実行メソッド</param>
        public ManholeToolCommand(Func<bool>? canExecuteHandler, Action? executeHandler)
        {
            this.canExecuteHandler = canExecuteHandler;
            this.executeHandler = executeHandler;
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="canExecuteHandler">コマンドが実行可能かどうか示す値を返す関数</param>
        /// <param name="executeHandler">コマンドの実行メソッド</param>
        public ManholeToolCommand(System.Func<object, bool> canExecuteHandler, System.Action<object> executeHandler)
        {
            this.canExecuteParaHandler = canExecuteHandler;
            this.executeParaHandler = executeHandler;
        }

        /// <summary>
        /// コマンドを実行できるかどうかを変更する可能性のある条件が System.Windows.Input.CommandManager によって検出された場合に発生します。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが実行可能かどうか示す値を返す関数を実行します。
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            var can = false;
            can = this.canExecuteHandler!();

            return can;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute(object? parameter)
        {
            this.executeHandler!();
        }
    }
} 