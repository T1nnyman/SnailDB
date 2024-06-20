import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'Home',
            component: () => import('../views/Home_View.vue')
        },
        {
            path: '/relics',
            name: 'Relics',
            component: () => import('../views/Relics_View.vue')
        },
        {
            path: '/relic/:id',
            name: 'Relic',
            component: () => import('../views/Relic_View.vue'),
            props: true
        },
        {
            path: '/gear',
            name: 'Gear',
            component: () => import('../views/Gears_View.vue'),
            props: true
        },
        {
            path: '/compass',
            name: 'Compass',
            component: () => import('../views/Compass_View.vue'),
            props: true
        },
        {
            path: '/museum',
            name: 'Museum',
            component: () => import('../views/Museum_View.vue'),
            props: true
        }
    ]
})

export default router