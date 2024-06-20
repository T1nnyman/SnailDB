<script setup>
    import { RouterLink } from 'vue-router'
    import { ref } from 'vue'

    const isMenuOpen = ref(false)

    const toggleMenu = () => {
        isMenuOpen.value = !isMenuOpen.value
    }
</script>

<template>
    <header class="mx-auto w-full bg-[#16181c]">
        <div class="pt-4 mx-auto hidden md:flex h-16 max-w-[1200px] items-center justify-around">
            <div class="flex items-center">
                <RouterLink to="/" class="flex items-center text-white" aria-label="Home Page">
                    <img alt="Vue logo" class="logo" src="@/assets/logo.svg" width="32" height="32" />
                    <span class="hidden text-lg font-semibold md:inline-block">SnailDB</span>
                </RouterLink>
            </div>
            <div class="ml-4 mr-4 flex items-center">
                <nav class="flex text-md ml-4 mr-4 gap-4 font-bold">
                    <RouterLink
                        to="/relics"
                        class="duration-250 text-decoration-none group relative transform text-[#b0bac5] transition-transform hover:scale-105 hover:text-white">
                        Relics
                        <span
                            class="scaleX-0 duration-250 absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform bg-[#1bd96a] transition-transform ease-in-out group-hover:scale-x-100"></span>
                    </RouterLink>
                    <RouterLink
                        to="/gear"
                        class="duration-250 text-decoration-none group relative transform text-[#b0bac5] transition-transform hover:scale-105 hover:text-white">
                        Gear
                        <span
                            class="scaleX-0 duration-250 absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform bg-[#1bd96a] transition-transform ease-in-out group-hover:scale-x-100"></span>
                    </RouterLink>
                    <RouterLink
                        to="/compass"
                        class="duration-250 text-decoration-none group relative transform text-[#b0bac5] transition-transform hover:scale-105 hover:text-white">
                        Compass
                        <span
                            class="scaleX-0 duration-250 absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform bg-[#1bd96a] transition-transform ease-in-out group-hover:scale-x-100"></span>
                    </RouterLink>
                    <RouterLink
                        to="/museum"
                        class="duration-250 text-decoration-none group relative transform text-[#b0bac5] transition-transform hover:scale-105 hover:text-white">
                        Museum
                        <span
                            class="scaleX-0 duration-250 absolute bottom-0 left-0 h-0.5 w-full scale-x-0 transform bg-[#1bd96a] transition-transform ease-in-out group-hover:scale-x-100"></span>
                    </RouterLink>
                </nav>
            </div>
            <div class="flex items-center gap-2">
                <a href="https://supersnail.wiki.gg/wiki/Super_Snail_Wiki" target="_blank" class="group">
                    <div
                        class="flex flex-row gap-2 rounded-xl border-2 border-gray-400 pb-1 pl-2 pr-2 pt-1 font-bold text-gray-400 group-hover:border-white group-hover:text-white">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <g id="SVGRepo_bgCarrier" stroke-width="0" />
                            <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round" />
                            <g id="SVGRepo_iconCarrier">
                                <path
                                    class="stroke-gray-400 group-hover:stroke-white"
                                    d="M4 19V6.2C4 5.0799 4 4.51984 4.21799 4.09202C4.40973 3.71569 4.71569 3.40973 5.09202 3.21799C5.51984 3 6.0799 3 7.2 3H16.8C17.9201 3 18.4802 3 18.908 3.21799C19.2843 3.40973 19.5903 3.71569 19.782 4.09202C20 4.51984 20 5.0799 20 6.2V17H6C4.89543 17 4 17.8954 4 19ZM4 19C4 20.1046 4.89543 21 6 21H20M9 7H15M9 11H15M19 17V21"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round" />
                            </g>
                        </svg>
                        Wiki
                    </div>
                </a>
                <a href="https://github.com/T1nnyman/SnailDB" target="_blank" class="group">
                    <div
                        class="flex flex-row gap-2 rounded-xl border-2 border-gray-400 pb-1 pl-2 pr-2 pt-1 font-bold text-gray-400 group-hover:border-white group-hover:text-white">
                        <svg
                            width="24px"
                            height="24px"
                            viewBox="0 0 20 20"
                            version="1.1"
                            xmlns="http://www.w3.org/2000/svg"
                            xmlns:xlink="http://www.w3.org/1999/xlink">
                            <g id="Page-1" fill="none">
                                <g id="Dribbble-Light-Preview" transform="translate(-140.000000, -7559.000000)" class="fill-gray-400 group-hover:fill-white">
                                    <g id="icons" transform="translate(56.000000, 160.000000)">
                                        <path
                                            d="M94,7399 C99.523,7399 104,7403.59 104,7409.253 C104,7413.782 101.138,7417.624 97.167,7418.981 C96.66,7419.082 96.48,7418.762 96.48,7418.489 C96.48,7418.151 96.492,7417.047 96.492,7415.675 C96.492,7414.719 96.172,7414.095 95.813,7413.777 C98.04,7413.523 100.38,7412.656 100.38,7408.718 C100.38,7407.598 99.992,7406.684 99.35,7405.966 C99.454,7405.707 99.797,7404.664 99.252,7403.252 C99.252,7403.252 98.414,7402.977 96.505,7404.303 C95.706,7404.076 94.85,7403.962 94,7403.958 C93.15,7403.962 92.295,7404.076 91.497,7404.303 C89.586,7402.977 88.746,7403.252 88.746,7403.252 C88.203,7404.664 88.546,7405.707 88.649,7405.966 C88.01,7406.684 87.619,7407.598 87.619,7408.718 C87.619,7412.646 89.954,7413.526 92.175,7413.785 C91.889,7414.041 91.63,7414.493 91.54,7415.156 C90.97,7415.418 89.522,7415.871 88.63,7414.304 C88.63,7414.304 88.101,7413.319 87.097,7413.247 C87.097,7413.247 86.122,7413.234 87.029,7413.87 C87.029,7413.87 87.684,7414.185 88.139,7415.37 C88.139,7415.37 88.726,7417.2 91.508,7416.58 C91.513,7417.437 91.522,7418.245 91.522,7418.489 C91.522,7418.76 91.338,7419.077 90.839,7418.982 C86.865,7417.627 84,7413.783 84,7409.253 C84,7403.59 88.478,7399 94,7399"
                                            id="github-[#142]"></path>
                                    </g>
                                </g>
                            </g>
                        </svg>
                        GitHub
                    </div>
                </a>
            </div>
        </div>
        <!-- Mobile Navbar -->
        <div class="pt-4 mx-auto flex items-center justify-around md:hidden">
            <div class="flex items-center">
                <RouterLink to="/" class="flex items-center text-white" aria-label="Home Page">
                    <img alt="Vue logo" class="logo" src="@/assets/logo.svg" width="32" height="32" />
                    <span class="text-lg font-semibold inline-block">SnailDB</span>
                </RouterLink>
            </div>
            <div class="flex items-center">
                <button @click="toggleMenu" class="stroke-white hover:stroke-[#1bd96a] transition duration-200">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16m-7 6h7"></path>
                    </svg>
                </button>
            </div>
        </div>
        <div v-if="isMenuOpen" class="fixed inset-0 z-50 bg-[#16181c] bg-opacity-95">
            <div class="flex justify-end p-4">
                <button @click="toggleMenu" class="text-white">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                </button>
            </div>
            <nav class="flex flex-col items-center space-y-4 mt-8 text-lg font-semibold">
                <RouterLink to="/relics" @click="toggleMenu" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    Relics
                </RouterLink>
                <RouterLink to="/gear" @click="toggleMenu" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    Gear
                </RouterLink>
                <RouterLink to="/compass" @click="toggleMenu" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    Compass
                </RouterLink>
                <RouterLink to="/museum" @click="toggleMenu" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    Museum
                </RouterLink>
                <a href="https://supersnail.wiki.gg/wiki/Super_Snail_Wiki" @click="toggleMenu" target="_blank" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    Wiki
                </a>
                <a href="https://github.com/T1nnyman/SnailDB" @click="toggleMenu" target="_blank" class="text-[#b0bac5] hover:text-white hover:underline hover:scale-105 transition duration-250 transform">
                    GitHub
                </a>
            </nav>
        </div>
    </header>
</template>
