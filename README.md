# NewYouID

[UUIDv7 generator for .NET 5](https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01#section-4.4)

Generate 128-bit sequential IDs like a boss with this .NET library.

Sample use:

```
var g = new Generator();
var id = g.NextId();
```

By default, a random identifier is created for each `Generator` instantiated. If you'd like to control you generator identifier, you can do this:

```
var g = new Generator(0x1BADBEEF);
var id = g.NextId();
```