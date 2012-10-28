
This is a for from http://stashy.codeplex.com/

Stash data simply.
==================

Stashy is a Micro-NoSql framework.

Stashy is a tiny interface, plus four reference implementations, for adding data storage and retrieval to all your .net types.

Drop a single stashy class into your application then you can save and load the lot.
How to use Stashy

Download and run the sample MVC3 app, to get a feel for what it does.
Or grab one of the implementations and drop it into your own application.
Then you can save, load, delete or query for your own types. No db needed.
How to implement your own Stash

For any implementation of IStashy, you need to tell it what type of Keys you will use -- for example Int or Guid.
Then you need to implement these five simple methods:
void Save<T1>(T1 t1, K id);

Given an instance of some type, save it somewhere.
T1 Load<T1>(K id) where T1 : new();

Given a type and a key value, return the requested item.
IEnumerable<T1> LoadAll<T1>() where T1 : new();

Given a type, return all known instances of it. (Same as 'Select * from' some table)
void Delete<T1>(K id);

Given a type and a key, delete it. (Delete from some table where Key = id.)
K GetNewId<T1>() where T1 : new();

Given a type, T1, return a new key. For example, If you're using sequential keys, you would have to return the next number in the series. If you're using Guids return a new guid. Implementation details.
Last edited Jun 8, 2011 at 3:20 PM by secretGeek, version 3
