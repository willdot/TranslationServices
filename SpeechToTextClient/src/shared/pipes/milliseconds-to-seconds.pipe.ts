import { Pipe, PipeTransform } from '@angular/core';

@Pipe ({
    name: 'millisecondsToSeconds'
})

export class MillisecondsToSecondsPipe implements PipeTransform {
    transform(val: number ): number {
        return val / 1000;
    }
}
