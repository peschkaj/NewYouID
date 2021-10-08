# NewYouID

[RFC 4122 (draft) UUIDv7](https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01#section-4.4) generator for .NET 5

Generate 128-bit sequential IDs like a boss with this .NET library.

Sample use:

```
var g = UuidGeneratorFactory.CreateUuidGenerator(UuidGeneratorFactory.Precision.Millisecond);
var id = g.NextId();
```

By default, a random identifier is created for each `Generator` instantiated. If you'd like to control you generator identifier, you can do this:

```
var g =UuidGeneratorFactory.CreateUuidGenerator(UuidGeneratorFactory.Precision.Millisecond, 0x1BADBEEF);
var id = g.NextId();
```
