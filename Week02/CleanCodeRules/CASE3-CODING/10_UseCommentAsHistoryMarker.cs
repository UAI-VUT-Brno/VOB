// Clean code case: comments are not a changelog - keep history in version control, not scribbled above methods.
// Let the code state what it does today and rely on git for the timeline, otherwise comments drift out of sync.

// BAD

// 2025-28-10 Kovar Created file
// 2025-28-10 Kroupa modified method
// ...

if (student.IsActive())
{}


// BETTER

if (student.IsActive())
{}
