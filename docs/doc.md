# CASC Documention (EN)  

## Introduction  

CASC is a handwritten compiler which have syntaxes for both Englsih and Manderin. It is:  

* Strong typing
* Highly customizable syntax typing (Mixed Manderin and English code is valid)

CASC is basically an improved JavaScript with statically typing.

This documention will take you around CASC programming language's features, once you finish the documention, you mostly learned the entire CASC programming language's features.

## Installation

Currently we don't provide user a easy installation process, but you can still get it from CASC GitHub repository's Releases. Once you downloaded it from Releases, there'll be a file called CASC-Compiler.exe inside the zip file.

Though CASC do have a repl for users to test out syntaxes, but the output of the repl is pretty much a disaster for now, if you are encountering n output issue, type /clear to clear the entire console.



## Table of Contents

<table>
    <tr><td width=33% valign=top>

* [Hello World](#hello-world)
    
    </td></tr>
</table>

## Hello World

Pure English Syntax: 
```casc
func main() {
    print("Hello world!")
}
```

Pure Manderin Syntax:
```casc
函式 main() {
    print("你好世界！")
}
```

Simple enough, let's start from the first line of code: 
`func` / `函式` declares a function, and `main()` defines an entry point for CASC compiler to execute, you don't define a main function while typing in repl.

`print()` function is one of builtin function, it takes one string as arugment. `print()` prints the string provided in argument.
