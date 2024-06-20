<template>
    <div class="p-4">
        <div v-if="isLoading" class="flex items-center justify-center">
            <img src="@/assets/loading.gif" alt="Loading" class="h-16 w-16" />
        </div>
        <div v-else-if="!relic" class="flex items-center justify-center">
            <p class="rounded-md bg-red-900 p-4 text-center text-white">Failed to load / Invalid Relic</p>
        </div>
        <div v-else class="mb-4 rounded-md bg-[#1c1f24] p-4 lg:p-6 xl:p-8">
            <button title="back" @click="navigateBack" class="group flex flex-row gap-2 rounded-md bg-gray-700 px-4 py-2 lg:px-6 lg:py-3 xl:px-8 xl:py-4">
                <svg width="25px" height="25px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M6 12H18M6 12L11 7M6 12L11 17"
                        stroke="#fff"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="group-hover:stroke-green-300" />
                </svg>
            </button>
            <div class="">
                <div class="w-full rounded-lg">
                    <img :src="'/assets/relics/' + relic.imageUri" alt="Relic Image" class="h-32 w-full object-contain md:h-40 lg:h-48 xl:h-56" />
                    <p class="mt-2 w-full text-center font-bold md:text-2xl lg:text-3xl xl:text-4xl">
                        {{ relic.name }}
                    </p>
                    <div class="mt-2 flex flex-row items-center justify-center">
                        <p :class="rankColorClass" class="m-2 rounded-full px-4 text-white lg:px-6 xl:px-8">
                            {{ relic.rank }}
                        </p>
                        <p class="m-2 rounded-full bg-gray-500 px-4 lg:px-6 xl:px-8">{{ relic.type }}</p>
                    </div>
                    <div v-if="relic.skills" class="flex flex-col items-center justify-center">
                        <div class="mt-2 flex flex-row justify-center gap-4">
                            <img
                                v-for="skill in relic.skills"
                                :key="skill.skill"
                                :src="'/assets/relics/' + skill.badgeUri"
                                alt="Relic Image"
                                class="object-contain md:w-12 lg:w-16 xl:w-20" />
                        </div>
                        <div class="mt-2">
                            <ul class="text-center text-xs text-white md:text-sm lg:text-base xl:text-lg">
                                <li v-for="skill in relic.skills" :key="skill.skill">• {{ skill.skill }}</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="flex w-full flex-col justify-center rounded-lg">
                    <div class="">
                        <div class="mx-auto max-w-md rounded-lg p-2 lg:p-4 xl:p-6">
                            <div class="mt-2 flex flex-col items-center justify-center">
                                <button
                                    v-if="maxStars > 0 && relic.rank !== 'Green'"
                                    @click="selectedStar = 'Awakened'"
                                    :class="[selectedStar === 'Awakened' ? rankColorClass : 'bg-gray-600 ' + ('hover:' + rankColorClass + '/50')]"
                                    class="mx-1 w-fit rounded-full px-4 py-2 font-bold text-white lg:px-6 lg:py-3 xl:px-8 xl:py-4">
                                    Awakened
                                </button>
                                <div class="mt-2 flex justify-center">
                                    <button
                                        v-for="star in maxStars"
                                        :key="star"
                                        @click="selectedStar = star"
                                        :class="[selectedStar === star ? rankColorClass : 'bg-gray-600 ' + ('hover:' + rankColorClass + '/50')]"
                                        class="mx-1 rounded-full px-4 py-2 font-bold text-white lg:px-6 lg:py-3 xl:px-8 xl:py-4">
                                        {{ star }}
                                    </button>
                                </div>
                            </div>
                            <div name="currentStats" class="mt-4 flex justify-between">
                                <div class="flex-1 text-center">
                                    <h3 class="font-bold text-white lg:text-xl xl:text-2xl">Art</h3>
                                    <p class="text-white lg:text-lg xl:text-xl">
                                        {{ currentStats ? currentStats.art : 'N/A' }}
                                    </p>
                                </div>
                                <div class="flex-1 text-center">
                                    <h3 class="font-bold text-white lg:text-xl xl:text-2xl">Fame</h3>
                                    <p class="text-white lg:text-lg xl:text-xl">
                                        {{ currentStats ? currentStats.fame : 'N/A' }}
                                    </p>
                                </div>
                                <div class="flex-1 text-center">
                                    <h3 class="font-bold text-white lg:text-xl xl:text-2xl">Faith</h3>
                                    <p class="text-white lg:text-lg xl:text-xl">
                                        {{ currentStats ? currentStats.faith : 'N/A' }}
                                    </p>
                                </div>
                                <div class="flex-1 text-center">
                                    <h3 class="font-bold text-white lg:text-xl xl:text-2xl">Civ</h3>
                                    <p class="text-white lg:text-lg xl:text-xl">
                                        {{ currentStats ? currentStats.civ : 'N/A' }}
                                    </p>
                                </div>
                                <div class="flex-1 text-center">
                                    <h3 class="font-bold text-white lg:text-xl xl:text-2xl">Tech</h3>
                                    <p class="text-white lg:text-lg xl:text-xl">
                                        {{ currentStats ? currentStats.tech : 'N/A' }}
                                    </p>
                                </div>
                            </div>
                        </div>
                        <ul class="flex flex-col items-center justify-center text-sm text-white lg:text-base xl:text-lg">
                            <li v-for="special in currentStats?.special" :key="special">
                                {{ special }}
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="mt-4 flex flex-col items-center justify-center">
                <div class="w-full">
                    <h2 class="text-center font-bold text-white underline md:text-lg lg:text-xl xl:text-2xl">Sources</h2>
                    <ul class="flex flex-col items-center justify-center text-sm text-white lg:text-base xl:text-lg">
                        <li v-for="source in relic.source?.split(', ')" :key="source">• {{ source }}</li>
                    </ul>
                </div>
                <div v-if="relic.description !== ''" class="mt-4 w-full rounded-lg">
                    <h2 class="text-center font-bold text-white underline md:text-lg lg:text-xl xl:text-2xl">Description</h2>
                    <p class="mx-auto max-w-screen-sm text-center text-sm text-white lg:text-base xl:text-lg">
                        {{ relic.description }}
                    </p>
                </div>
            </div>
            <div class="mt-4 rounded-lg bg-gray-500 p-2 lg:p-4 xl:p-6">
                <p class="text-center text-sm text-white lg:text-base xl:text-lg">
                    Note: Resonances and Mirages will be added if I get enough requests. Everything is already setup to handle them I would just need to expand this
                    page.
                </p>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { ref, onMounted, computed } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import { useRelicStore } from '@/stores/relicStore'
    import type { Relic, RelicStats } from '@/types/Relic'

    const store = useRelicStore()
    const isLoading = ref(false)
    const serverFailed = ref(false)
    const relic = ref<Relic | null>()
    const route = useRoute()
    const router = useRouter()
    const selectedStar = ref<number | string>()

    const starLevels: { [key: string]: number } = {
        Green: 3,
        Blue: 4,
        A: 5,
        AA: 5,
        AAA: 5,
        S: 6,
        SS: 6,
        SSS: 6,
    }

    const returnUrl = computed(() => {
        const { selectedGrade, selectedType, searchTerm, currentPage } = route.query
        return {
            name: 'Relics',
            query: {
                selectedGrade: selectedGrade || '',
                selectedType: selectedType || '',
                searchTerm: searchTerm || '',
                currentPage: currentPage || '',
            },
        }
    })

    const maxStars = computed(() => {
        if (relic.value) {
            return starLevels[relic.value.rank] || 0
        }
        return 0
    })

    const currentStats = computed<RelicStats | null>(() => {
        if (relic.value) {
            if (selectedStar.value === 'Awakened') {
                return relic.value.stats['Awakened'] || null
            }
            return relic.value.stats[Number(selectedStar.value)] || null
        }
        return null
    })

    const rankColorClass = computed(() => {
        if (!relic.value) return ''
        switch (relic.value.rank) {
            case 'Green':
                return 'bg-green-500'
            case 'Blue':
                return 'bg-blue-500'
            case 'A':
            case 'AA':
            case 'AAA':
                return 'bg-purple-500'
            case 'S':
            case 'SS':
            case 'SSS':
                return 'bg-orange-500'
            default:
                return ''
        }
    })

    onMounted(async () => {
        const relicId = route.params.id

        if (store.relics.length > 0) {
            relic.value = store.relics.find((relic) => relic.id === Number(relicId))
            selectedStar.value = relic.value?.rank === 'Green' ? 3 : 'Awakened'
        } else {
            isLoading.value = true
            try {
                const success = await store.fetchRelics()
                serverFailed.value = !success
                if (success) {
                    relic.value = store.relics.find((relic) => relic.id === Number(relicId))
                    selectedStar.value = relic.value?.rank === 'Green' ? 3 : 'Awakened'
                }
            } catch (error) {
                console.error('Error fetching relic:', error)
                serverFailed.value = true
            } finally {
                isLoading.value = false
            }
        }
    })

    const navigateBack = () => {
        router.push(returnUrl.value)
    }
</script>
