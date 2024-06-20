<template>
    <div class="p-4">
        <div class="mb-4 rounded-md bg-[#1c1f24] p-4">
            <div name="filteringOptions" class="flex flex-col gap-4 sm:flex-row">
                <div>
                    <label for="grade" class="text-sm font-medium text-white">Grade</label>
                    <select
                        id="grade"
                        v-model="selectedGrade"
                        @change="setSelectedGrade(selectedGrade)"
                        class="white w-full rounded-md bg-[#131518] py-2 pl-3 pr-10">
                        <option value="">All</option>
                        <option value="Green">Green</option>
                        <option value="Blue">Blue</option>
                        <option value="A">A</option>
                        <option value="AA">AA</option>
                        <option value="AAA">AAA</option>
                        <option value="S">S</option>
                        <option value="SS">SS</option>
                        <option value="SSS">SSS</option>
                    </select>
                </div>
                <div>
                    <label for="type" class="text-sm font-medium text-white">Type</label>
                    <select id="type" v-model="selectedType" @change="setSelectedType(selectedType)" class="white w-full rounded-md bg-[#131518] py-2 pl-3 pr-10">
                        <option value="">All</option>
                        <option value="ART">ART</option>
                        <option value="FAME">FAME</option>
                        <option value="FTH">FTH</option>
                        <option value="CIV">CIV</option>
                        <option value="TECH">TECH</option>
                    </select>
                </div>
                <div class="w-full">
                    <label for="search" class="text-sm font-medium text-white">Search</label>
                    <input
                        id="search"
                        v-model="searchTerm"
                        @input="setSearchTerm(searchTerm)"
                        type="text"
                        placeholder="(leadership, demon minion, hard stats)"
                        class="w-full rounded-md bg-[#131518] py-2 pl-3 pr-10 text-white truncate" />
                </div>
            </div>
        </div>
        <div v-if="isLoading" class="flex items-center justify-center">
            <img src="@/assets/loading.gif" alt="Loading" class="h-16 w-16" />
        </div>
        <div v-else-if="serverFailed" class="flex items-center justify-center">
            <p class="text-white">Failed to fetch relics. Please try again later.</p>
        </div>
        <div v-else-if="!isLoading && filteredRelics.length === 0" class="flex items-center justify-center">
            <p class="rounded-md bg-red-900 p-4 text-center text-white">No relics found, try adjusting your search criteria!</p>
        </div>
        <div v-else class="grid grid-cols-2 justify-items-center gap-4 text-center sm:grid-cols-3 md:m-2 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6">
            <div v-for="relic in paginatedRelics" :key="relic.id" class="w-full rounded-md bg-[#1c1f24] p-2">
                <router-link
                    :to="{
                        name: 'Relic',
                        params: { id: relic.id },
                        query: {
                            selectedGrade: selectedGrade,
                            selectedType: selectedType,
                            searchTerm: searchTerm,
                            currentPage: currentPage,
                        },
                    }"
                    class="block">
                    <img :src="'/assets/relics/' + relic.imageUri" alt="Relic Image" class="h-32 w-full object-contain" />
                    <div class="mt-2 w-full truncate font-bold hover:underline">
                        {{ relic.name }}
                    </div>
                </router-link>
                <div class="flex flex-col items-center">
                    <div class="flex flex-row items-center justify-center">
                        <p
                            :class="{
                                'bg-green-500': relic.rank === 'Green',
                                'bg-blue-500': relic.rank === 'Blue',
                                'bg-purple-500': ['A', 'AA', 'AAA'].includes(relic.rank),
                                'bg-orange-500': ['S', 'SS', 'SSS'].includes(relic.rank),
                            }"
                            class="m-1 rounded-full px-4 text-white md:m-2">
                            {{ relic.rank }}
                        </p>
                        <p class="m-1 rounded-full bg-gray-500 px-4 md:m-2">{{ relic.type }}</p>
                    </div>
                </div>
            </div>
        </div>
        <div class="mt-4 flex justify-center">
            <button @click="prevPage" :disabled="currentPage === 1" class="group rounded-l-md bg-gray-700 px-4 py-2">
                <svg width="25px" height="25px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M6 12H18M6 12L11 7M6 12L11 17"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        :class="currentPage > 1 ? 'group-hover:stroke-green-300' : ''"
                        :stroke="currentPage == 1 ? '#999' : '#fff'" />
                </svg>
            </button>
            <input type="number" v-model="currentPage" @change="goToPage" class="w-16 bg-gray-800 px-4 py-2 text-center text-white no-arrows" :min="1" :max="totalPages" />
            <button @click="nextPage" :disabled="currentPage >= totalPages" class="group rounded-r-md bg-gray-700 px-4 py-2">
                <svg width="25px" height="25px" viewBox="0 0 24 24" fill="" xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M6 12H18M18 12L13 7M18 12L13 17"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        :class="currentPage < totalPages ? 'group-hover:stroke-green-300' : ''"
                        :stroke="currentPage >= totalPages ? '#999' : '#fff'" />
                </svg>
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { onMounted, ref, computed, onBeforeUnmount } from 'vue'
    import { useRelicStore } from '@/stores/relicStore'
    import type { Relic } from '@/types/Relic'
    import { useRoute, useRouter } from 'vue-router'

    const store = useRelicStore()
    const isLoading = ref(false)
    const relics = ref<Relic[]>([])
    const route = useRoute()
    const router = useRouter()
    const serverFailed = ref(false)

    // Filtering options
    const selectedGrade = ref<string>(route.query.selectedGrade?.toString() || '')
    const selectedType = ref<string>(route.query.selectedType?.toString() || '')
    const searchTerm = ref<string>(route.query.searchTerm?.toString() || '')

    // Pagination
    const currentPage = ref(Number(route.query.currentPage) || 1)
    const itemsPerPage = ref(18) // default 3 rows of 6 relics, will be updated dynamically


    onMounted(async () => {
        updateItemsPerPage()
        window.addEventListener('resize', updateItemsPerPage)

        if (store.relics.length > 0) {
            relics.value = store.relics
        } else {
            isLoading.value = true
            try {
                const success = await store.fetchRelics()
                serverFailed.value = !success
                if (success) {
                    relics.value = store.relics
                }
            } catch (error) {
                console.error('Error fetching relics:', error)
                serverFailed.value = true
            } finally {
                isLoading.value = false
            }
        }
    })

    onBeforeUnmount(() => {
        window.removeEventListener('resize', updateItemsPerPage)
    })

    const setSearchTerm = (value: string) => {
        searchTerm.value = value
        currentPage.value = 1
        updateRoute()
    }

    const setSelectedGrade = (value: string) => {
        selectedGrade.value = value
        currentPage.value = 1
        updateRoute()
    }

    const setSelectedType = (value: string) => {
        selectedType.value = value
        currentPage.value = 1
        updateRoute()
    }

    const updateRoute = () => {
        const { fullPath } = router.currentRoute.value
        const query = {
            selectedGrade: selectedGrade.value || null,
            selectedType: selectedType.value || null,
            searchTerm: searchTerm.value || null,
            currentPage: currentPage.value || null,
        }
        router.push({ path: fullPath, query })
    }

    const updateItemsPerPage = () => {
        const width = window.outerWidth
        // default 18 items per page 3 rows of 6 relics for 1080p screens
        // Want to keep a similar ammount of relics per page for different screen sizes while keeping full rows
        // wll account for all screen sizes
        if (width <= 640) {
            itemsPerPage.value = 6
        } else if (width <= 768) {
            itemsPerPage.value = 9
        } else if (width <= 1024) {
            itemsPerPage.value = 12
        } else if (width <= 1280) {
            itemsPerPage.value = 15
        } else if (width <= 1920) {
            itemsPerPage.value = 18
        } else {
            itemsPerPage.value = 24
        }
    }

    const scrollToTop = () => {
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    const nextPage = () => {
        if (currentPage.value < totalPages.value) {
            currentPage.value++
            updateRoute()
            scrollToTop()
        }
    }

    const prevPage = () => {
        if (currentPage.value > 1) {
            currentPage.value--
            updateRoute()
            scrollToTop()
        }
    }

    const goToPage = () => {
        if (currentPage.value < 1) {
            currentPage.value = 1
        } else if (currentPage.value > totalPages.value) {
            currentPage.value = totalPages.value
        }
        updateRoute()
        scrollToTop()
    }

    // Computed property for filtered relics
    const filteredRelics = computed(() => {
        return relics.value.filter((relic) => {
            const matchesGrade = selectedGrade.value === '' || relic.rank === selectedGrade.value
            const matchesType = selectedType.value === '' || relic.type === selectedType.value
            const matchesSearch =
                searchTerm.value === '' ||
                relic.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                (relic.stats &&
                    Object.values(relic.stats).some(
                        (stat) =>
                            Array.isArray(stat.special) && stat.special.some((specialStat) => specialStat.toLowerCase().includes(searchTerm.value.toLowerCase()))
                    ))

            return matchesGrade && matchesType && matchesSearch
        })
    })

    const paginatedRelics = computed(() => {
        const start = (currentPage.value - 1) * itemsPerPage.value
        const end = start + itemsPerPage.value
        return filteredRelics.value.slice(start, end)
    })

    const totalPages = computed(() => {
        return Math.ceil(filteredRelics.value.length / itemsPerPage.value)
    })
</script>

<!-- Hide Number inputs arrows -->
<style>
.no-arrows::-webkit-inner-spin-button,
.no-arrows::-webkit-outer-spin-button {
    -webkit-appearance: none;
    margin: 0;
}
</style>