# Menu setup and editing

The Admin and Chef dashboards can create menu categories, add dishes with descriptions and prices, and edit existing dishes. On a fresh setup, sign in as an admin or chef and choose **Add Whisk-Takers sample menu** / **Load sample menu**. This adds missing sample dishes without duplicating existing ones.

The API applies pending migrations automatically in Development. In other environments, from the `backend` directory, apply pending migrations before starting the API:

```powershell
dotnet ef database update --project WhiskTakers.Infrastructure/WhiskTakers.Infrastructure.csproj --startup-project WhiskTakers.API/WhiskTakers.API.csproj
```

Menu changes are available to the client and waiter screens. Product create, edit, and delete API operations require an Admin or Chef account.

The Admin floor plan now supports named, color-coded areas, movable tables, and table resizing. Floor-plan changes sync to the API for other staff devices; if the API is offline, the current browser copy remains available. Add an area, select its tab, and use the grab handle to move tables or the lower-right handle to resize them.

The Admin floor plan shows whether it synced to the server and has a **Save layout** button. Waiter and Chef screens refresh the shared layout every 10 seconds. If it reports a sync error, check that the API is running and that its migrations were applied.

Set each dish's cost per serving and preparation time in Menu editor. The profit chart uses the price and cost saved on each order, so later cost edits do not rewrite past margins. Older orders have no recorded cost and are excluded until new orders are created with a cost set. The dotted what-if panel estimates profit at the same 30-day sales volume; it is a transparent calculation, not an AI forecast.

Waiters can tap a menu item to add one serving, then use plus/minus controls in the order list. Chefs get a dish timer based on the longest preparation time in an order, with +5/+10 minute extensions and an early-finish option. Timer start times and extensions are saved on the order so kitchen screens stay in sync.

## Staff access and menu photos

Choose Admin, Waiter, or Chef on the welcome screen, then sign in with the staff member's password only. Passwords must be unique for each staff account within its role. Sample credentials are Admin `admin123`, Waiter John `waiter123`, Waiter Sydney `sydney123`, and Chef `chef123`. Clients can open `/client` directly without an account.

Admin and Chef can set an image URL for each menu item while creating or editing it. If no URL is set, the client and waiter menus use a rotating set of sample food photos. The client page also includes a “Who we are” section with café photography and Albanian/English text.

The what-if card now waits for a positive scenario price and cost before showing a projection. Its value is `(scenario price − scenario cost) × completed portions sold in the last 30 days`. It only shows a change versus the current estimate when the dish has a current unit cost configured, so a missing cost cannot appear as a false loss.
