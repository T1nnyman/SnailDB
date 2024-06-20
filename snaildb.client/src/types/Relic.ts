export interface Relic {
    id: number;
    name: string;
    source?: string;
    description: string;
    imageUri: string;
    wikiUrl: string;
    type: string;
    rank: string;
    stats: { [key: string]: RelicStats };
    skills?: { [key: string]: RelicBadgeSkill };
}

export interface RelicStats {
    fame: string;
    art: string;
    faith: string;
    civ: string;
    tech: string;
    special: string[];
}

export interface RelicBadgeSkill {
    skill: string;
    badgeUri: string;
}