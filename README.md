# Hero-Tycoon-Mystic-Lands

---
# Working rules:
## Code:
All variables must define Access Modifiers (public, private, protected, ...)

Any public instance (class, variable, function, ...) must use `PascalCase`

All private/protected instance must use `_camelCase`

Local private variables must use `camelCase`
## Git: 
Git'll always have 3 main branches:
- Main: only update newest release version
- Develop: commit for all smaller developing branches

### To start working:
Create a new branch from branch `develop`, name new branch as: `dev/<user name>`
### To push commit:
Use `Git conversion` before specific task name

Ex1:

    Task: Create isometric map
    Commit: feat: create isometric map
Ex2:

    Task: Fix isometric map code
    Commit: fix: fix isometric map bug <detail>
### To merge commit:
    After working branch finish a task or a sum of tasks, only push to working branch 
    
        => Pull from branch `develop` to resolve conflict 
    
            => Resolve conflict(if any) and push merge to working branch 
    
                => Create `Merge request` from working branch to branch `develop`
### To build game:
Create new build branch from branch `develop`, name new branch as: `build/<version name>`

After build: 

    Update this `Readme` file's change log (create new version's change log)
    Create `Merge request` to branch `develop` to update `Readme`
