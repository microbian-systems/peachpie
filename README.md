# Peachpie Compiler
### The open-source PHP compiler to .NET
http://www.peachpie.io | https://twitter.com/pchpcompiler

_If you would like to reward us for our hard work on this project, we will be very happy to accept donations of all amounts._ [![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BY2V98VY57K2E)

## What is Peachpie?
Peachpie is a modern PHP compiler based on Roslyn by Microsoft and drawing from our popular Phalanger project. It allows PHP to be executed within the .NET framework, thereby opening the door for PHP developers into the world of .NET – and vice versa.

The goal of the project is to compile legacy PHP code into portable class libraries, which enables developers to build cross-platform apps and libraries for Microsoft platforms quickly and easily. As a direct result, PHP applications powered by Peachpie would run across all devices and operating systems that are able to run .NET.

Besides granting PHP programmers access to cross-platform development, Peachpie allows for a full compatibility with .NET, which enables the development of hybrid applications, where part of the code is written in C# and part in PHP. The parts will be entirely compatible and can communicate seamlessly, all within the .NET framework.

Peachpie makes use of Microsoft's Roslyn compiler and years worth of developing Phalanger, where the design for this compiler was established and valuable experience was obtained in the process. This, together with an extensive type analysis, allows us to set the objective of Peachpie to be a significant performance increase of PHP applications and components. 

## Status
Please note that the status is dynamic; Peachpie is a work in progress, which means that this list frequently changes and is updated on a regular basis. We are currently focused on providing a first demo.

###### Compiler:
   :white_check_mark: Microsoft.CodeAnalysis (Roslyn) (ILBuilder, ObjectPool, MetadataReader, ...)   
   :white_check_mark: Abstract Syntax Tree (AST)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: AST based on Roslyn SyntaxNode (optional)   
   :white_check_mark: Temporary parser (PHP5)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_medium_square: Parser (PHP7) based on PHP7 and SyntaxTree (optional)   
   :white_check_mark: Symbol tables, Metadata   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Source symbols   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: .NET references (for core and Packages)     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark:Types   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Methods, Parameters, Locals  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Overrides, implements interface member     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Fields     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Properties   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Interfaces   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Constants  
   :white_check_mark: Control Flow Graph (CFG), Binder (Roslyn IOperation)     
   :white_medium_square: SemanticModel (gets symbols to be used by emit)   
   :white_medium_square: GetTypeFromTypeRef  
   :white_medium_square: Code Analysis  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Type Analysis    
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: AST transformation   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Constant propagation   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Analyze subsequent string concatenations to optimize as one   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Value range analysis (value fits into Long, no need for Double)     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Inter-procedure analysis     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Security analysis     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Parallelization (as a distributed algorithm)           
   :white_medium_square: Code generation (Translation into .NET MSIL assembly)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Type system, conversions   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Routines declaration   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Types declaration   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Destructors (as Dispose)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Ignore empty destructors   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Magic methods  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Methods overriding   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Places (locals, context)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Blocks compiler   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Expressions emit         
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Routine call (and Operators)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Overload resolution   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Object instantiation   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Field setter/getter   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Array access   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Chains   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Indirects (var, func, const)     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Routine generalization    
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Application runtime tables  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Inclusion   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Function with variable arguments count (params)   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Traits (AST transformation: merge trait with a class declaration)     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Closures     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Generators     
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Generics     
   :white_check_mark: Portable class library   
   :white_medium_square: Console App: entry script invocation   
   :white_medium_square: Web App: request handler => Script invocation 

###### Runtime:
   :white_medium_square: Type system  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: Bool, Number (Long, Double), String, Object, Array, Resource, Null   
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Extended: UnicodeString, SimpleArray (not keyed, same type value)  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: PhpValue (Any Type runtime value)  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_check_mark: PhpAlias (aliasing, ref counting, destructors)  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Specific type (known at compile time)  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Lazy copy-on-write (array, string)  
   :white_medium_square: Operators  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Generic operator mechanism for given operands  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Numbers  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Arrays  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Strings (Unicode and binary)  
   :white_medium_square: Inclusion (static target, dynamic target)  
   :white_medium_square: Function call  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Ambiguous function call (the one resolved at runtime)  
   :white_medium_square: Context (actual state of program, global variables, scripts, visible functions and types)  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Fast/direct global variables access  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Delegates to actual functions  
   :white_medium_square: Output (echo, buffering)  
   :white_medium_square: Runtime Tables (PhpTypeInfo, PhpRoutineInfo, Scripts: bit vector everything indexed)  
   :white_medium_square: Dynamic invocation/read/write  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Functions  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Methods  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Variables  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Fields  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Magic methods  
   :white_medium_square: Loop constructs, Enumerables  
   :white_medium_square: Aliasing: PHPVALUE (is_ref, ref_count for references and objects with dtor)  
   :white_medium_square: Constants (global, class)  
   :white_medium_square: Static fields  
   :white_medium_square: Static variables  
   :white_medium_square: Reflection API  
   :white_medium_square: Exceptions, Exception handling  
   :white_medium_square: Destructors / Disposable  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Reference counting for locals  
   :white_medium_square: Inclusion in a local scope  
   :white_medium_square: PHP stream  
   :white_medium_square: Eval, create_function()  

###### API:
   :white_medium_square: Context  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Events (begin request, end request, start app, exit, ...)  
   :white_medium_square: Script invocation  
   :white_medium_square: Function call  
   :white_medium_square: Reflection (Runtime Tables, PHPDoc)  
   :white_medium_square: Code verification  
   :white_medium_square: Compilation  
   :white_medium_square: Global variables access  
###### Packages:
   :white_medium_square: Symbol tables of a referenced assembly  
   :white_medium_square: Migrate Phalanger Class Library  
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:white_medium_square: Strongly typed  
   :white_medium_square: Fast PCRE  
   :white_medium_square: SPL  
   :white_medium_square: Reflection  

## How to build or execute?
This project is a work in progress. Please be aware that Peachpie is not meant to be downloaded and executed at this point. Currently, this is a concept, which you can use for your inspiration, but it is not intended to be launched. 

## How to contribute?
We can use all the help we can get. You can contribute to our repository, spread the word about this project, or give us a small donation to help fund the development. If you believe you have valuable knowledge and experience to add to this project, please do not hesitate to contribute to our repo – your help is much appreciated.

## How to get in touch?
We kindly ask you to be patient with your queries; you can follow us on Twitter @pchpcompiler or on Facebook. You can contact us there regarding your questions, but please understand that we do not provide support at this point. 
