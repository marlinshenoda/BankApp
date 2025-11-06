## 🧠 Overview
A simple Responsive-Blazor-based banking app for managing accounts, transactions, deposits, withdrawals, and transfers.  
Built as part of the YH project in .NET 8 using Blazored.LocalStorage.
---

## ⚙️ Core Features 
- Create and manage multiple bank accounts
- Deposit, withdraw, and transfer money(Change balances & record transactions)
- View recent transactions(Filter and sort transactions)
- Persistent data in LocalStorage
- User sign-in system
- Apply Interest to Savings
- Budget/Expense Categories (e.g. Food, Rent, Transport)

 🖥 Pages
 1️⃣ Sign In Page (/signin)(+User enters username and PIN Calls +SignInService.SignInAsync() +If successful → navigates to Home)
 2️⃣ Home Page (/)(+Welcome message +Total balance across accounts +Chart (balances per account)
    +Recent Transaction List with:✅ Search✅ Category filter✅ Status filter✅ Date range✅ Sorting)
 3️⃣ My Accounts Page (/accounts)(+Displays cards for each account. +Each account has action buttons:Deposit,Withdraw,Transfer and Apply Interest (Saving Only) +A modal pops up to enter:
Amount, Description (optional)and Category)
 4️⃣ Transfer Page (/transaction)(+ A dedicated transfer form(deposit , Withdraw and transfer))
 5️⃣ Connect Bank (/connectbank)( +create a new bank account)

<img width="1872" height="908" alt="image" src="https://github.com/user-attachments/assets/d786c03c-3b4d-4244-a641-5d512844ff3e" />

<img width="1865" height="851" alt="image" src="https://github.com/user-attachments/assets/0f46f9aa-fd11-4570-bd97-c5a3345e9afe" />
