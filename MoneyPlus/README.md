# MoneyPlus

-> GitHub Repository: https://github.com/inesestevesdeataide/MoneyPlus

**IN THIS FOLDER**

-> DbScripts folder: set of csv files exported from the current working database in ADS numbered by a appropriate order of insertion. Tables Category, Subcategory weren't included assuming you'll be working with the catSubcatList yaml file. Table DailyEmail is authomatically filled, so the file won't be in the folder either. innie@mail.com is the most active user in that DB (Password: -Uz$5nh7s3p7aD-)

-> catSubcatList.yaml
   Path to read from yaml file in use:
    var file = "/Users/inesestevesdeataide/catSubcatList.yaml";
    Equivalent for Windows (commented in the file): var file = "c:\\temp\\catSubcatList.yaml";
    
    
**MoneyPlus Project**

-> appSettings.json 
Connection String in use:
    "DefaultConnection": "Data Source=localhost;Database=moneyPlus_local;User=SA;Password=FranklyMyDear00.sql;Integrated Security=False;MultipleActiveResultSets=true;Connection Timeout=3;TrustServerCertificate=True"
Possible Connection String for Windows users (commented in the file):
    "DefaultConnection": "Data Source=.\\;Initial Catalog=moneyPlus_local;Integrated Security=True;MultipleActiveResultSets=true;Connection Timeout=360;TrustServerCertificate=True;"
    
    
* DESIGN DECISIONS 
- Record Types
    We decided to create different entities to represent the different operations being recorded. Cash Outflows and Inflows refer only to expenses and incomes that may or may be not associated with assets. On the other hand, investments are about financial transactions that generate a new Wallet that pertains to the instrument on which the investment falls. The entity Transfer is about operations between different wallets and selling investments. Each category is associated to a single record type.
- Soft Delete
    As some entities work as foreign keys to other entities (Asset, Payee, Wallet, Category, Subcategory) and deleting them would crash the database during the usage of the application, we decided to use booleans to control if those entities are active or not. If they become inactive, they will not be seen by the users anylonger but are still working foreign keys for previous entries. The only partial exception is for Categories and Subcategories. For users they work as aforementioned. However, the admin is able to see all active and not active categories ans subcategories, managing their status in the reserved admin page.
- Negative Values
    As in the financial world, we assumed that wallets and assets may at some point have a positive or negative value. Contrarily, all records, as each represent a different directional cash flows may only assume positive values. 
- Unique Registers
    We only allow the creation and edition of Asset, Payee, Wallet, Category and Subcategory if they are not duplicates for a given user. Subcategories can have the same name as long as they do not belong to the same category. This uniqueness rule does not consider previously (soft) deleted assets, payees ans wallets - meaning, after deletion of an asset "Mansion", you can register "Mansion" again.
- Definition of Wealth
    We considered the sum of all assets and wallets' values to be a real user's wealth at a given moment. Since assets are transactionable and a possible source of income, according to Accounting rules, this is a more accurate measure of wealth/patrimony than merely taking into account their wallets' balances. Therefore, the calculations in R6 and R10 reflect this decision.
- Definition of Average Cost Of Living
    To have a more realistic and trustworthy picture, according to Economics and Accounting, we took into consideration the average living cost over the past 24 months for R6 calculations.
- Assigning Admin Role 
    Besides the query string method, there is also an admin page requiring a password ("pouco-segura") to grant access to the secret page.
    When assining Admin role, if the user already exists, we opted not to allow the alteration of the account's password in that process. Assuming anyone with access to the validated query string may know your email address, if the password was changed indeed, you would be locked out of your account. For security reasons, we thought this would not be a desired outcome.


* KNOWN PROBLEMS 
- When trying to add some registers with an invalid field it is possible that the page does not reload properly. Please navigate backwards on your browser.
- Incomplete loggers of critical errors and exceptions
    Due to lack of time and for prioritizing the completion of the assigned main feature set, we were only able to implement some loggers in the razor pages of Assets, Payees and Wallets.
- Incomplete Repositories 
    After some initial time-consuming concerns regarding Mac compatibility between Visual Studio and SqlServer, our main focus was to manage the built-in CRUD feature effectively in order to be able to vertically test the integration from the page up to the database. After starting with this approach, repositories were kind of sidelined and we ended up implementing them on entities Asset, Payee and Wallet and partially on Cash Outflows. Nonetheless, we recognize they make the code cleaner and easier to follow and change.
- Selling Investments
    Even though we wished we had handled capital gains and losses in a traditional manner, since Wallets can be edited by the user, that was not possible. So, in case of a capital gain, the whole initial investment may follow back to its inital origin wallet (or other, if chosen) and the gains to another. In case of capital loss, the amount received for the selling transaction may be pushed to a wallet to be chosen by the user. Had we deviated the loss to a wallet, that would be penalizing the user's wealth, so we decided to stick with letting the accounting right.


* WEB APP INSTRUCTIONS

Under the menu Your Records, you'll find six different sections where you'll be able to register, consult and update your inputs.

Firstly, section Assets refers to your tangible assets, Property, Plant and Equipment (PP&E). Here, you can include any PP&E you wish naming it as you see fit and assigning it a value. However, be alerted: you mustn't have multiple assets under the same name. Later, these assets can be associated with your cash outflows, if you decide so.

In the section Payees, you get to make a list of entities you frequently buy from. It's up to you whether you want to include their Tax Number or not. But again, make sure they all have different names.

Next, your Wallets. We assume you have money in different places. For example, one or two bank accounts, a piggy bank saving a few coins for some purchase you've been dreaming of, or even a stash of bills under your mattress for a rainy day. All these can be a wallet in Money+. If you have some sort of Emergency Fund, you can make a wallet out of it.

A wallet can be the subject of a withdrawal or a deposit any time you want. To do so, you must resort to a Cash Outflow or Inflow record, respectivelly. In addition, you can optionally make transfers directly from one wallet to another. Just go to the Wallets section and proceed to transfer. By the way, it's worth mentioning all these sections open with a dashboard displaying all your registers so far.

Whenever you have an expense, don't forget to register it in the Cash Outflows section. First you'll see a global statement of all your cash outflows (as aforementioned), from the most recent to the oldest. In the upper left corner of your screen you will find a blue button “Add New” and there you'll be able to enter your record.

To do this, you will be asked to choose one of your previously registered Payees or you can select “Or register a new Payee” from there. Afterwards, you must select your origin wallet, enter the amount of your outflow, the date and choose the subcategory in which it falls. Optionally, you can write a short description or associate it with an asset you own. By this, we mean Asset "Beach Home", for example, may be part of a cash outflow when you're buying it or when it makes you spend some money on reparations. In case you'd like to update or rectify your outflow later, just go to the Cash Outflows section, find the desired outflow and click on "Edit".

As for Cash Inflows, no big news. The procedures are pretty much the same. Everything is very intuitive on Money+ and we're sure you'll know exactly how to move around there.

Last, but not least, under the menu Your Records, you'll find a Financial Investments section. This section is reserved to financial transactions that bring you some sort of intangible possession. There aren't major differences to the the other record types except in this case you'll have to select an origin wallet (money always leaves from somewhere, right?) and name your destination wallet (i.e., the wallet that consubstanciates your investmente). For example, if you were investing on Money+ stocks you could call that destination wallet "Money+ Stocks" or "I own part of the coolest enterprise in the world". It's absolutely up to you. 

Plus, when you sell your investment we move the money back to different wallets for you. Just click Sell, make your choices and we'll deal with the rest.

Under a different menu, Your Reports, you'll be able to consult several reports we have provided so you can have a more conscious and realistic view of your finances. As of today, you'll be able to find:
    - An Yearly Balance of your spendings by category 
    - A Monthly Balance of outflows based on category and subcategory in a given year
    - A Monthly Listing of your detailed outflows that you can filter by being associated to a certain asset, belonging to a given category or having been made with some payee
    - A simulator of how many years off work you get to have given your current personal wealth, your expectations of future inflation and return on investment, and your average cost of life over the past 2 years.
