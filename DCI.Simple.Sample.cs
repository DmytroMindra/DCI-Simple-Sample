using System;

namespace DCI.Simple.Sample
{

   public abstract class Account
   {
      public double Balance { get; protected set; }

      public void Withdraw(double amount)
      {
         Balance -= amount;
      }

      public void Deposit(double amount)
      {
         Balance += amount;
      }

      public void Log(string message)
      {
         Console.WriteLine("Log: {0}", message);
      }
   }

   public interface TransferMoneySink
   {
      void Deposit(double amount);
      void Log(string message);
   }

   public interface TransferMoneySource
   {
      double Balance { get; }
      void Withdraw(double amount);
      void Log(string message);
   }

   public class CheckingAccount : Account, TransferMoneySource, TransferMoneySink
   {
      public CheckingAccount()
      {
         Balance = 1000;
      }

      public override string ToString()
      {
         return "Balance " + Balance;
      }
   }

   public class SavingsAccount : Account, TransferMoneySource, TransferMoneySink
   {
      public SavingsAccount()
      {
         Balance = 1000;
      }

      public override string ToString()
      {
         return "Balance " + Balance;
      }
   }

   public static class TransferMoneySourceTrait
   {
      public static void TransferTo(this TransferMoneySource self, TransferMoneySink recipient, double amount)
      {
         if (self.Balance < amount)
         {
            throw new ApplicationException("insufficient funds");
         }

         self.Withdraw(amount);
         self.Log("Withdrawing " + amount);
         recipient.Deposit(amount);
         recipient.Log("Depositing " + amount);
      }
   }
   
   public class TransferMoneyContext
   {
      public TransferMoneySource Source { get; private set; }
      public TransferMoneySink Sink { get; private set; }
      public double Amount { get; private set; }

      public TransferMoneyContext(TransferMoneySource source,
          TransferMoneySink sink, double amount)
      {
         Source = source;
         Sink = sink;
         Amount = amount;
      }

      public void Execute()
      {
         Source.TransferTo(Sink, Amount);
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         // получаем данные
         var savingsAccount = new SavingsAccount();
         var checkingAccount = new CheckingAccount();

         OutputBalances(savingsAccount, checkingAccount);
         Console.Write("\n");

         // создаем контекст и фиксируем данные в определенных ролях
         TransferMoneyContext context = new TransferMoneyContext(savingsAccount,checkingAccount,500);

         // выполняем контекст
         context.Execute();

         OutputBalances(savingsAccount, checkingAccount);
         Console.Write("\n");
      }
      
      static void OutputBalances(SavingsAccount savingsAccount, CheckingAccount checkingAccount)
      {
         Console.WriteLine("Savings Balance: {0:c}", savingsAccount.Balance);
         Console.WriteLine("Checking Balance: {0:c}", checkingAccount.Balance);
      }
   }



}
