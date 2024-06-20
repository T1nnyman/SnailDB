import { defineStore } from 'pinia'
import type { Relic } from '@/types/Relic';

interface RelicsState {
    isLoading: boolean;
    relics: Relic[];
}

export const useRelicStore = defineStore('relics', {
    state: (): RelicsState => ({
        isLoading: false,
        relics: [],
    }),
    actions: {
        async fetchRelics(): Promise<boolean> {
            this.isLoading = true;
            if (this.relics.length > 0) {
                return true;
            }
            try {
                const response = await fetch('/api/Relic/GetRelics'); // Relative to the server root, replace with localhost:5173/api/Relic/GetRelics during development
                const data = await response.json();
                this.relics = data;
                return true;
            } catch (error) {
                console.error('Error fetching relics:', error);
                return false;
            } finally {
                this.isLoading = false;
            }
        },
    },
});