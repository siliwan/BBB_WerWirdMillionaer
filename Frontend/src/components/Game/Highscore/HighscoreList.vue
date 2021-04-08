<template>
    <div class="highscoreList container">
        <b-table 
            id="highscoreListTable"
            striped 
            hover 
            outlined
            responsive
            :items="highscores" 
            isBusy="isLoading" 
            :fields="fields"
            :select-mode="'single'"
            selectable
            :per-page="perPage"
            :current-page="currentPage"
            @row-selected="onRowSelected">
        </b-table>
        <b-pagination
        v-model="currentPage"
        :total-rows="highscores.length"
        :per-page="perPage"
        aria-controls="highscoreListTable" />
    </div>
</template>
<script lang="ts">
    import HighscoreCrud from "@/components/CRUD/HighscoreCrud";
    import { HighScore, nameof } from "@/ResponseTypes";
    import { Component, Prop, Vue, Watch } from 'vue-property-decorator';

    @Component({
        components: {
            
        }
    })
    export default class HighscoreList extends Vue {

        private warningText: string = '';
        private highscores: HighScore[] = [];
        private isLoading: boolean = true;

        private currentPage: number = 0;
        private perPage: number = 10;

        private fields = [
            { key: nameof<HighScore>("rank"), label: "Rank", sortable: true },
            { key: nameof<HighScore>("name"), label: "Name" },
            { key: nameof<HighScore>("pointsWeighted"), label: "Score", sortable: true },
            { key: nameof<HighScore>("categories"), label: "Categories", formatter: this.onCategoriesFormat },
        ]

        constructor() {
            super();
        }

        async mounted() {
            await this.reload();
        }

        async reload() {
            this.isLoading = true;
            this.highscores = await HighscoreCrud.getAll();
            this.isLoading = false;
        }

        onRowSelected(item: HighScore) {
            if(Array.isArray(item)) {
                item = item[0];
            }

            this.$router.push(`/highscores/${item.id}`);
        }

        onCategoriesFormat(currentValue: string, fieldKey: any, item: HighScore) {
            return item.categoriesSeperated ?? item.categories.split(',').join(', ');
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
