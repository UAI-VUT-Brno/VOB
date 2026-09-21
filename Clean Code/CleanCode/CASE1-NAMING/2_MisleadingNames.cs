// Clean code case: avoid misleading collection names by naming what the data represents.
// Readers should instantly know they're looking at orders, not a mysterious pile of "objects".

// Bad
var allObjects = db.GetFromTable.ToList();
    
// Better
var listOfOrders = _orderService.GetOrders().ToList();
