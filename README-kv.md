Project is forked from [CodePlex hg repo] (http://kv.codeplex.com/)

__Kv -?__

kv -- a command-line key-value store integrated with the clipboard.
inspired by: https://github.com/stevenleeg/boo

usage:

__kv name fred smith__
saves the the value, 'fred smith' under the key, 'name'

__kv name__
retrieve the value 'fred smith' straight to your clipboard.

__kv__
lists all keys

__kv h*__ 
lists all keys that match the pattern 'h*'

__kv -r name__
will remove the key ‘name’ (and its value) from your store

You can also pipe a value in, e.g.

echo Hello Fred | kv Greeting
will store 'Hello Fred' under the key 'Greeting'
type File.xml | kv myFile
will store the content of 'File.xml' under the key 'myFile'
Keys are case-insensitive.

Where does it store the data?

Each key and value is stored in a separate file, in the folder %localappdata%\kv\kv.snippet\
Each file in that folder is named after its key (encoded so that special characters are supported)