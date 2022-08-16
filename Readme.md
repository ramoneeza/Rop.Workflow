# Rop.Workflow

Features
--------

Rop.Workflow is a simple Workflow Engine written in C# and NET 6.0.

The package is published in nuget as 'Rop.Workflow'

Usage
-----

Inherits the BaseWorkflow abstract class and Provide FirstStep as a Method in your Workflow class.

Each Step is a Method with o whitout a CancellationToken and a parameter.
The return of each Step is a NextStatus item.

A Saga is the fork of a Workflow in several paralelle processes.

A Defer is a external call to make an action in a controlled thread. 
It is userful to work with UI Threats.

IWorkflowPersistence IoC allows serialization of a Workflow into a Database or File.
This alows to stop and resume a Workflow and his persistence across multiple executions.


 ------
 (C)2022 Ramón Ordiales Plaza
