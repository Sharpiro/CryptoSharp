import { trigger, state, transition, animate, style, keyframes } from '@angular/animations'

export type flashState = 'greenflash' | 'redflash' | ''

export const flashAnimation =
    trigger('flash', [
        state('greenflash', style({ backgroundColor: 'white' })),
        transition('* => greenflash', [
            animate(500, keyframes([
                style({ backgroundColor: 'lightgreen', offset: 0.5 }),
                style({ backgroundColor: 'white', offset: 1.0 }),
            ]))
        ]),
        state('redflash', style({ backgroundColor: 'white' })),
        transition('* => redflash', [
            animate(500, keyframes([
                style({ backgroundColor: 'red', offset: 0.5 }),
                style({ backgroundColor: 'white', offset: 1.0 }),
            ]))
        ])
    ])