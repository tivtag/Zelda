
# Shows the given [text] ontop of the given [speaker] for [duration] seconds.
def bubble_text(text, duration = 9, speaker = nil)
    scene.BubbleTextManager.ShowText(text, duration, (ee speaker))
end

# Gets a value indicating whether any bubble text is currently shown
def conversation_in_progress()
  scene.BubbleTextManager.ActiveTextCount > 0
end

# Changes the current entity for the duration of the given [block] of code.
def on(entity)
    old_entity = @current_entity
    @current_entity = entity
    
    yield
       
    @current_entity = old_entity
end

# ensure_entity
def ee(entity)
    if entity.nil? then
        @current_entity
    else
        entity
    end
end

# Executes the given block; once for the current scene.
# This per-scene state change is persisted.
def once(identifier)
    if not_set(identifier) then 
        yield
        set identifier
    end
end

def entity(entity_name)
	scene.GetEntity(entity_name)
end

def get_entity(entity_name)
	scene.GetEntity(entity_name)
end

# Executes the given block; once for the current scene.
# Changing the current entity to the given entity for the given block.
# The per-scene state change is persisted.
def once_on(entity, identifier)
    once identifier do
        on entity do
            yield
        end
    end
end

# Executes the given block; once temporarily until the game is reloaded.
# This per-world state change is not persisted.
def once_temp(identifier)
    if not_set_temp(identifier) then 
        yield
        set_temp identifier
    end
end


##
## Spawning
##

# Spawns a decorative entity at the given spawn point
# Example: 
# deco = spawn_deco 'PoisonCloud_A', 'BFS_3',
#     type: 'anim_1D',
#     anim_speed: 500, 
#     on_anim_ended: 'despawn',
#     colored: 'randomly'  
#
def spawn_deco(sprite_name, spawnpoint_name, hash)
    deco = dsl.SpawnDeco(sprite_name, spawnpoint_name, hash[:type])
        
    if hash.key? :anim_speed then
        deco.DrawDataAndStrategy.CurrentAnimation.AnimationSpeed = hash[:anim_speed]
    end
    
    if hash[:on_anim_ended] == 'despawn' then
        despawn_on_animation_end deco
    end
    
    if hash[:colored] == 'randomly' then
        deco.DrawDataAndStrategy.BaseColor = dsl.GetRandomColorRGB()    
    end
        
    deco
end

# Spawns enemies of a given type at the given spawn point [count] times.
# Example:
#    on get_entity('ESP_House_0') do
#        spawn_enemy 'Skeleton', 10
#        spawn_enemy 'Skeleton_Red', 5
#        spawn_enemy 'Skeleton_Green', 3
#    end
def spawn_enemy(template_name, count = 1, spawn_point = nil)
   spawn_point = ee(spawn_point)

   dsl.SpawnEnemy(template_name, spawn_point, count)
end

def despawn_on_animation_end(entity)
    dsl.DespawnOnAnimationEnd(entity)
end

def despawn(entity)
	dsl.Despawn(entity)
end

# Spawns the given or the current entity at the spawn point with the given name
#
# Example:
# on player do
#    spawn_at 'SP_TestSpawnPoint'
# end
def spawn_at(spawn_point_name, entity = nil)
	dsl.SpawnAt(spawn_point_name, ee(entity))
end


##
## Events
##

# Executes the given block once after [seconds].
#
# seconds - The ingame real-time to wait before [block] is executed.
#
def after_seconds(seconds, &block)
    dsl.AfterSeconds(seconds, block)
end


# Executes the given block when a specific event occurred.
#
# event_id - id that identifies the event to listen to
#            supported events:
#                :level-up   -   called when the player has gained a level
# 
# block    - If true is returned the event will be unregistered.
#
def on_event(event_id, &block)
    dsl.OnEvent(event_id.to_s, block)
end


##
## Light
##

## Changes the background light color of the scene
## This has only an effect with scenes that use ambient light.
##
def set_ambient_color(red, green, blue, alpha=255)
  dsl.SetAmbientColor(red, green, blue, alpha)  
end


##
## Day Night Cycle
##

# Executes the given block when an ingame 'time event'
# is fired. 
#
# block - Passed the DayNightEvent enumeration: DayBegan, DayEnded, EveningBegan, EveningEnded, NightBegan, NightEnded
#		  If true is returned the event will be unregistered.
#
def on_time_changed(&block)
  scene.DayNightCycle.AddOnceEvent block  
end


##
## Weather
##

def has_weather()
    scene.WeatherMachine.HasActiveWeather
end

def set_weather(type)
    if type == :rain_storm then
        scene.WeatherMachine.SetWeatherByTypeName( 'Zelda.Weather.Creators.RainStormCreator' )
    else
        false
    end
end

def hint_weather(type)
    if not has_weather then
        set_weather type
    else
        set_weather type
    end
end


##
## Tiles
##

def change_tiles(floor_number, layer_index, source_tile, target_tile, target_action_tile)
	scene.Map.ChangeTiles(floor_number, layer_index, source_tile, target_tile, target_action_tile)
end

def change_tiles_2(floor_number, layer_index, source_tileA, target_tileA, target_action_tileA, source_tileB, target_tileB, target_action_tileB)
	scene.Map.ChangeTiles(floor_number, layer_index, source_tileA, target_tileA, target_action_tileA, source_tileB, target_tileB, target_action_tileB)
end

def set_tile_action(floor_number, tile_x, tile_y, target_action_tile)
	dsl.SetActionTile(floor_number, tile_x, tile_y, target_action_tile)
end


##
## Audio
##

def play_sample(sound_name, volume = 1.0)
    sample = dsl.PlaySample(sound_name, volume)
end

def play_music(sound_name)
    dsl.PlayMusic(sound_name)
end


## 
## Value Storage
##

# Gets a value indicating whether a value has been
# persisted in the current scene.
def has_value(identifier)
    scene_status.DataStore.Contains(identifier)
end

# Gets the boolean value with the given identifier string
# that has been persisted in the current scene.
def get_bool(identifier)
    storage = scene_status.DataStore.TryGetBoolean(identifier)
    
    if storage.nil? then
        false
    else
        storage.Value
    end
end

# Sets the boolean value with the given id to the given value for the current scene.
def set_bool(identifier, initial_value=true)
    scene_status.DataStore.CreateBoolean(identifier, initial_value)
end

# Gets a value indicating whether the given boolean identifier has not been set.
def not_set(identifier)
    not (get_bool identifier)
end

# Gets a value indicating whether the given boolean identifier has not been set
# in the temporary world storage.
def not_set_temp(identifier)
    not (get_temp_bool identifier)
end

def set(identifier)
    set_bool identifier
end

# Gets the boolean value with the given identifier string
# that has been set in temporary world storage.
def get_temp_bool(identifier)
    storage = world_status.TempDataStore.TryGetBoolean(identifier)
    
    if storage.nil? then
        false
    else
        storage.Value
    end
end

# Sets the boolean value with the given id to the given value temporarily in world data storage
def set_temp_bool(identifier, initial_value=true)
    world_status.TempDataStore.GetOrCreateBoolean(identifier, initial_value)
end

def set_temp(identifier)
    set_temp_bool identifier
end


##
## Random
##

## Gets a random value between (including) 0 and 255 
##
def random_byte()
  dsl.GetRandomByte()
end
