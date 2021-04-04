<template>
    <div class="highscoreList container">
        <b-table 
            striped 
            hover 
            outlined
            responsive
            :items="highscores" 
            isBusy="isLoading" 
            :fields="fields"
            :select-mode="'single'"
            selectable
            @row-selected="onRowSelected">
        </b-table>
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

        private fields = [
            { key: nameof<HighScore>("rank"), label: nameof<HighScore>("rank"), sortable: true },
            { key: nameof<HighScore>("name"), label: nameof<HighScore>("name") },
            { key: nameof<HighScore>("pointsWeighted"), label: "points", sortable: true },
            { key: nameof<HighScore>("categories"), label: nameof<HighScore>("categories"), formatter: this.onCategoriesFormat },
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
