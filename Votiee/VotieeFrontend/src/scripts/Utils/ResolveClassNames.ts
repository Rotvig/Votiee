module Utils {
    export class ResolveClassNames {
        resolve(...classnames: string[]): string {

            var namesStringToReturn = "";
            _.each(classnames, (className) => {
                if (className !== "" && className !== undefined && className !== null) {
                    namesStringToReturn += className + " ";
                }

            });
            return namesStringToReturn.trim();
        }
    }
} 